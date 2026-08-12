using BlogPlatform.Data;
using BlogPlatform.Models;
using BlogPlatform.Models.Enums;
using BlogPlatform.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Services
{
    public class InteractionService : IInteractionService
    {
        private readonly BlogDbContext context;

        public InteractionService(BlogDbContext context)
        {
            this.context = context;
        }

        public async Task<ToggleResultViewModel> ToggleLikeAsync(int postId, int userId)
        {
            var post = await context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null || post.Status != PostStatus.Published)
            {
                return new ToggleResultViewModel
                {
                    IsSuccess = false,
                    Message = "Bài viết không tồn tại hoặc chưa công khai.",
                    IsActive = false,
                    NewCount = 0
                };
            }

            var existingLike = await context.PostLikes
                .FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == userId);

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                bool isActive;
                if (existingLike != null)
                {
                    context.PostLikes.Remove(existingLike);
                    if (post.LikeCount > 0)
                    {
                        post.LikeCount -= 1;
                    }
                    isActive = false;
                }
                else
                {
                    var newLike = new PostLike
                    {
                        PostId = postId,
                        UserId = userId,
                        CreatedAt = DateTime.Now
                    };
                    context.PostLikes.Add(newLike);
                    post.LikeCount += 1;
                    isActive = true;
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ToggleResultViewModel
                {
                    IsSuccess = true,
                    Message = string.Empty,
                    IsActive = isActive,
                    NewCount = post.LikeCount
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ToggleResultViewModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi thực hiện thao tác thích bài viết: {ex.Message}",
                    IsActive = false,
                    NewCount = post.LikeCount
                };
            }
        }

        public async Task<ToggleResultViewModel> ToggleBookmarkAsync(int postId, int userId)
        {
            var post = await context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null || post.Status != PostStatus.Published)
            {
                return new ToggleResultViewModel
                {
                    IsSuccess = false,
                    Message = "Bài viết không tồn tại hoặc chưa công khai.",
                    IsActive = false,
                    NewCount = 0
                };
            }

            var existingBookmark = await context.Bookmarks
                .FirstOrDefaultAsync(b => b.PostId == postId && b.UserId == userId);

            try
            {
                bool isActive;
                if (existingBookmark != null)
                {
                    context.Bookmarks.Remove(existingBookmark);
                    isActive = false;
                }
                else
                {
                    var newBookmark = new Bookmark
                    {
                        PostId = postId,
                        UserId = userId,
                        CreatedAt = DateTime.Now
                    };
                    context.Bookmarks.Add(newBookmark);
                    isActive = true;
                }

                await context.SaveChangesAsync();

                return new ToggleResultViewModel
                {
                    IsSuccess = true,
                    Message = string.Empty,
                    IsActive = isActive,
                    NewCount = 0
                };
            }
            catch (Exception ex)
            {
                return new ToggleResultViewModel
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi lưu bài viết: {ex.Message}",
                    IsActive = false,
                    NewCount = 0
                };
            }
        }

        public async Task<bool> IsLikedAsync(int postId, int userId)
        {
            return await context.PostLikes
                .AsNoTracking()
                .AnyAsync(pl => pl.PostId == postId && pl.UserId == userId);
        }

        public async Task<bool> IsBookmarkedAsync(int postId, int userId)
        {
            return await context.Bookmarks
                .AsNoTracking()
                .AnyAsync(b => b.PostId == postId && b.UserId == userId);
        }

        public async Task<List<PostListItemViewModel>> GetUserBookmarksAsync(int userId)
        {
            return await context.Bookmarks
                .AsNoTracking()
                .Include(b => b.Post)
                    .ThenInclude(p => p!.Author)
                .Include(b => b.Post)
                    .ThenInclude(p => p!.Category)
                .Where(b => b.UserId == userId && b.Post != null && b.Post.Status == PostStatus.Published)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new PostListItemViewModel
                {
                    Id = b.Post!.Id,
                    Title = b.Post.Title,
                    Slug = b.Post.Slug,
                    Summary = b.Post.Summary,
                    FeaturedImageUrl = b.Post.FeaturedImageUrl,
                    AuthorId = b.Post.AuthorId,
                    AuthorUserName = b.Post.Author != null ? b.Post.Author.UserName : string.Empty,
                    AuthorDisplayName = b.Post.Author != null && !string.IsNullOrWhiteSpace(b.Post.Author.DisplayName) ? b.Post.Author.DisplayName : (b.Post.Author != null ? b.Post.Author.UserName : string.Empty),
                    CategoryName = b.Post.Category != null ? b.Post.Category.Name : null,
                    CategorySlug = b.Post.Category != null ? b.Post.Category.Slug : null,
                    Status = b.Post.Status,
                    PublishedAt = b.Post.PublishedAt,
                    ViewCount = b.Post.ViewCount,
                    LikeCount = b.Post.LikeCount,
                    CommentCount = b.Post.CommentCount
                })
                .ToListAsync();
        }
    }
}

