using BlogPlatform.Data;
using BlogPlatform.Models;
using BlogPlatform.Models.Enums;
using BlogPlatform.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Services
{
    public class CommentService : ICommentService
    {
        private readonly BlogDbContext context;
        private readonly IHtmlSanitizerService htmlSanitizerService;

        public CommentService(BlogDbContext context, IHtmlSanitizerService htmlSanitizerService)
        {
            this.context = context;
            this.htmlSanitizerService = htmlSanitizerService;
        }

        public async Task<List<CommentViewModel>> GetTreeByPostAsync(int postId, int? currentUserId)
        {
            // 1 Query duy nhất lấy tất cả comment Approved (hoặc Pending của chính người đang đăng nhập)
            var rawComments = await context.Comments
                .AsNoTracking()
                .Include(c => c.User)
                .Where(c => c.PostId == postId && (
                    c.Status == CommentStatus.Approved ||
                    (currentUserId.HasValue && c.UserId == currentUserId.Value && c.Status == CommentStatus.Pending)
                ))
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            var vmMap = new Dictionary<int, CommentViewModel>();
            foreach (var c in rawComments)
            {
                vmMap[c.Id] = new CommentViewModel
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    DisplayName = !string.IsNullOrWhiteSpace(c.User?.DisplayName) ? c.User.DisplayName : (c.User?.UserName ?? "Người dùng"),
                    AvatarUrl = c.User?.AvatarUrl,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    Status = c.Status,
                    Level = 1,
                    Replies = new List<CommentViewModel>()
                };
            }

            var rootList = new List<CommentViewModel>();

            foreach (var c in rawComments)
            {
                var vm = vmMap[c.Id];
                if (c.ParentCommentId.HasValue && vmMap.TryGetValue(c.ParentCommentId.Value, out var parentVm))
                {
                    if (parentVm.Level >= 3)
                    {
                        vm.Level = 3;
                    }
                    else
                    {
                        vm.Level = parentVm.Level + 1;
                    }
                    parentVm.Replies.Add(vm);
                }
                else
                {
                    vm.Level = 1;
                    rootList.Add(vm);
                }
            }

            return rootList;
        }

        public async Task<string> CreateAsync(int postId, int userId, int? parentCommentId, string content)
        {
            // 1. Kiểm tra bài viết tồn tại và đã Published
            var post = await context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null || post.Status != PostStatus.Published)
            {
                return "Bài viết không tồn tại hoặc chưa được công khai";
            }

            // 2. Kiểm tra và sanitize nội dung
            if (string.IsNullOrWhiteSpace(content))
            {
                return "Nội dung bình luận không được để trống";
            }

            var sanitizedContent = htmlSanitizerService.SanitizeCommentContent(content.Trim());
            if (string.IsNullOrWhiteSpace(sanitizedContent))
            {
                return "Nội dung bình luận không hợp lệ";
            }

            // 3. Xử lý ParentCommentId & Giới hạn tối đa 3 cấp
            int? targetParentId = null;
            if (parentCommentId.HasValue && parentCommentId.Value > 0)
            {
                var parentComment = await context.Comments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == parentCommentId.Value && c.PostId == postId);

                if (parentComment == null)
                {
                    return "Bình luận trả lời không tồn tại";
                }

                if (parentComment.ParentCommentId.HasValue)
                {
                    var grandParent = await context.Comments
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == parentComment.ParentCommentId.Value);

                    if (grandParent != null && grandParent.ParentCommentId.HasValue)
                    {
                        // Comment cha đã ở cấp 3 -> Gắn cùng ParentCommentId với comment cha để giữ ở cấp 3
                        targetParentId = parentComment.ParentCommentId;
                    }
                    else
                    {
                        targetParentId = parentComment.Id;
                    }
                }
                else
                {
                    targetParentId = parentComment.Id;
                }
            }

            // 4. Quyết định trạng thái: Tác giả tự bình luận bài mình -> Approved; Độc giả -> Pending
            var status = (post.AuthorId == userId) ? CommentStatus.Approved : CommentStatus.Pending;

            // 5. Lưu DB trong Transaction (Cập nhật CommentCount nếu Approved)
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var comment = new Comment
                {
                    PostId = postId,
                    UserId = userId,
                    ParentCommentId = targetParentId,
                    Content = sanitizedContent,
                    Status = status,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                context.Comments.Add(comment);

                if (status == CommentStatus.Approved)
                {
                    post.CommentCount += 1;
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return "SUCCESS";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return $"Lỗi khi lưu bình luận: {ex.Message}";
            }
        }

        // ===== Implementation cho Issue #8 =====
        public async Task<List<CommentListItemViewModel>> GetPendingByAuthorAsync(int authorId)
        {
            var currentUser = await context.Users.AsNoTracking().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == authorId);
            bool isAdmin = currentUser?.Role?.Name == "Admin";

            var query = context.Comments
                .AsNoTracking()
                .Include(c => c.User)
                .Include(c => c.Post)
                .Include(c => c.ParentComment)
                .Where(c => c.Status == CommentStatus.Pending);

            if (!isAdmin)
            {
                query = query.Where(c => c.Post != null && c.Post.AuthorId == authorId);
            }

            var pendingComments = await query
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentListItemViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt,
                    UserId = c.UserId,
                    UserDisplayName = c.User != null && !string.IsNullOrWhiteSpace(c.User.DisplayName) ? c.User.DisplayName : (c.User != null ? c.User.UserName : "Người dùng"),
                    PostId = c.PostId,
                    PostTitle = c.Post != null ? c.Post.Title : string.Empty,
                    PostSlug = c.Post != null ? c.Post.Slug : string.Empty,
                    PostAuthorId = c.Post != null ? c.Post.AuthorId : 0,
                    ParentExcerpt = c.ParentComment != null ? c.ParentComment.Content : null
                })
                .ToListAsync();

            return pendingComments;
        }

        public async Task<int> CountPendingAsync(int authorId)
        {
            var currentUser = await context.Users.AsNoTracking().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == authorId);
            bool isAdmin = currentUser?.Role?.Name == "Admin";

            if (isAdmin)
            {
                return await context.Comments.AsNoTracking().CountAsync(c => c.Status == CommentStatus.Pending);
            }

            return await context.Comments
                .AsNoTracking()
                .CountAsync(c => c.Status == CommentStatus.Pending && c.Post != null && c.Post.AuthorId == authorId);
        }

        public async Task<string> ApproveAsync(int commentId, int currentUserId)
        {
            var comment = await context.Comments
                .Include(c => c.Post)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                return "Bình luận không tồn tại";
            }

            var currentUser = await context.Users.AsNoTracking().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == currentUserId);
            bool isAdmin = currentUser?.Role?.Name == "Admin";

            if (comment.Post == null || (comment.Post.AuthorId != currentUserId && !isAdmin))
            {
                return "Bạn không có quyền duyệt bình luận này";
            }

            if (comment.Status == CommentStatus.Approved)
            {
                return "Bình luận đã được duyệt trước đó";
            }

            bool wasApproved = comment.Status == CommentStatus.Approved;
            comment.Status = CommentStatus.Approved;
            comment.UpdatedAt = DateTime.Now;

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (!wasApproved && comment.Post != null)
                {
                    comment.Post.CommentCount += 1;
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return "SUCCESS";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return $"Lỗi khi duyệt bình luận: {ex.Message}";
            }
        }

        public async Task<string> RejectAsync(int commentId, int currentUserId)
        {
            var comment = await context.Comments
                .Include(c => c.Post)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                return "Bình luận không tồn tại";
            }

            var currentUser = await context.Users.AsNoTracking().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == currentUserId);
            bool isAdmin = currentUser?.Role?.Name == "Admin";

            if (comment.Post == null || (comment.Post.AuthorId != currentUserId && !isAdmin))
            {
                return "Bạn không có quyền từ chối bình luận này";
            }

            bool wasApproved = comment.Status == CommentStatus.Approved;
            comment.Status = CommentStatus.Rejected;
            comment.UpdatedAt = DateTime.Now;

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (wasApproved && comment.Post != null && comment.Post.CommentCount > 0)
                {
                    comment.Post.CommentCount -= 1;
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return "SUCCESS";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return $"Lỗi khi từ chối bình luận: {ex.Message}";
            }
        }

        public async Task<string> FlagAsync(int commentId, int currentUserId)
        {
            var comment = await context.Comments
                .Include(c => c.Post)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                return "Bình luận không tồn tại";
            }

            var currentUser = await context.Users.AsNoTracking().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == currentUserId);
            bool isAdmin = currentUser?.Role?.Name == "Admin";

            if (comment.Post == null || (comment.Post.AuthorId != currentUserId && !isAdmin))
            {
                return "Bạn không có quyền gắn cờ bình luận này";
            }

            bool wasApproved = comment.Status == CommentStatus.Approved;
            comment.Status = CommentStatus.Flagged;
            comment.UpdatedAt = DateTime.Now;

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (wasApproved && comment.Post != null && comment.Post.CommentCount > 0)
                {
                    comment.Post.CommentCount -= 1;
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return "SUCCESS";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return $"Lỗi khi gắn cờ bình luận: {ex.Message}";
            }
        }

        public async Task<string> DeleteAsync(int commentId, int currentUserId)
        {
            var comment = await context.Comments
                .Include(c => c.Post)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                return "Bình luận không tồn tại";
            }

            var currentUser = await context.Users.AsNoTracking().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == currentUserId);
            bool isAdmin = currentUser?.Role?.Name == "Admin";

            if (comment.Post == null || (comment.Post.AuthorId != currentUserId && !isAdmin))
            {
                return "Bạn không có quyền xóa bình luận này";
            }

            bool wasApproved = comment.Status == CommentStatus.Approved;

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (wasApproved && comment.Post != null && comment.Post.CommentCount > 0)
                {
                    comment.Post.CommentCount -= 1;
                }

                context.Comments.Remove(comment);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return "SUCCESS";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return $"Lỗi khi xóa bình luận: {ex.Message}";
            }
        }
    }
}

