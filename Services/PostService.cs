using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using BlogPlatform.Data;
using BlogPlatform.Models;
using BlogPlatform.Models.Enums;
using BlogPlatform.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Services
{
    // Nghiệp vụ quản lý bài viết — Issue #5
    // UC15: Tạo bài
    // UC16: Sửa bài
    // UC17: Xóa bài
    // UC18: Publish
    // UC19: Unpublish
    // UC21: Kiểm tra quyền sở hữu bài viết
    public class PostService : IPostService
    {
       private readonly BlogDbContext _context;
private readonly ITaxonomyService _taxonomyService;
private readonly IHtmlSanitizerService _htmlSanitizerService;

     public PostService(
    BlogDbContext context,
    ITaxonomyService taxonomyService,
    IHtmlSanitizerService htmlSanitizerService)
{
    _context = context;
    _taxonomyService = taxonomyService;
    _htmlSanitizerService = htmlSanitizerService;
}
        // =========================================================
        // Lấy danh sách bài viết của tác giả
        // =========================================================
        public async Task<List<Post>> GetByAuthorAsync(int authorId)
        {
            return await _context.Posts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.PostTags)
                    .ThenInclude(pt => pt.Tag)
                .Where(p => p.AuthorId == authorId)
                .OrderByDescending(p => p.UpdatedAt)
                .ToListAsync();
        }

        // =========================================================
        // Lấy bài viết theo Id
        // =========================================================
        public async Task<Post?> GetByIdAsync(int id)
        {
            return await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.Author)
                .Include(p => p.PostTags)
                    .ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // =========================================================
        // Tạo bài viết mới
        // =========================================================
        public async Task<Post> CreateAsync(
            PostEditViewModel model,
            int authorId)
        {
            var slug = await GenerateUniqueSlugAsync(model.Title);

            var post = new Post
            {
                Title = model.Title.Trim(),
                Slug = slug,
                Summary = string.IsNullOrWhiteSpace(model.Summary)
                    ? null
                    : model.Summary.Trim(),
                Content = _htmlSanitizerService.Sanitize(model.Content),
                FeaturedImageUrl = string.IsNullOrWhiteSpace(model.FeaturedImageUrl)
                    ? null
                    : model.FeaturedImageUrl.Trim(),
                CategoryId = model.CategoryId,
                AuthorId = authorId,

                // Quy tắc: bài mới luôn là Draft
                Status = PostStatus.Draft,

                PublishedAt = null,
                ViewCount = 0,
                LikeCount = 0,
                CommentCount = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Posts.Add(post);

            // Cần Save trước để Post có Id
            await _context.SaveChangesAsync();

            // Xử lý danh sách tag
            await UpdatePostTagsAsync(post, model.TagNames);

            await _context.SaveChangesAsync();

            return post;
        }

        // =========================================================
        // Cập nhật bài viết
        // =========================================================
        public async Task<bool> UpdateAsync(
            PostEditViewModel model,
            int authorId)
        {
            var post = await _context.Posts
                .Include(p => p.PostTags)
                .FirstOrDefaultAsync(p => p.Id == model.Id);

            if (post == null)
            {
                return false;
            }

            // UC21 — chỉ chủ bài viết mới được sửa
            if (post.AuthorId != authorId)
            {
                return false;
            }

            post.Title = model.Title.Trim();

            post.Summary = string.IsNullOrWhiteSpace(model.Summary)
                ? null
                : model.Summary.Trim();

         post.Content = _htmlSanitizerService.Sanitize(model.Content);

            post.FeaturedImageUrl =
                string.IsNullOrWhiteSpace(model.FeaturedImageUrl)
                    ? null
                    : model.FeaturedImageUrl.Trim();

            post.CategoryId = model.CategoryId;

            // Quy tắc:
            // Nếu bài đã Published thì KHÔNG đổi slug.
            // Điều này bảo vệ các URL đã được chia sẻ.
            if (post.Status != PostStatus.Published)
            {
                post.Slug = await GenerateUniqueSlugAsync(
                    post.Title,
                    post.Id);
            }

            post.UpdatedAt = DateTime.Now;

            // Cập nhật tags
            await UpdatePostTagsAsync(post, model.TagNames);

            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // Xóa bài viết
        // =========================================================
        public async Task<bool> DeleteAsync(
            int id,
            int authorId)
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return false;
            }

            // UC21 — chống IDOR
            if (post.AuthorId != authorId)
            {
                return false;
            }

            _context.Posts.Remove(post);

            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // Publish bài viết
        // =========================================================
        public async Task<bool> PublishAsync(
            int id,
            int authorId)
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return false;
            }

            // UC21
            if (post.AuthorId != authorId)
            {
                return false;
            }

            post.Status = PostStatus.Published;
            post.PublishedAt = DateTime.Now;
            post.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // Unpublish bài viết
        // =========================================================
        public async Task<bool> UnpublishAsync(
            int id,
            int authorId)
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return false;
            }

            // UC21
            if (post.AuthorId != authorId)
            {
                return false;
            }

            post.Status = PostStatus.Unpublished;
            post.UpdatedAt = DateTime.Now;

            // Giữ lại PublishedAt để biết bài từng được publish.
            // Nếu business rule của nhóm yêu cầu xóa PublishedAt
            // khi unpublish thì có thể đổi thành null.
            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // Cập nhật danh sách Tag của bài viết
        // =========================================================
        private async Task UpdatePostTagsAsync(
            Post post,
            string? tagNames)
        {
            // Xóa toàn bộ liên kết tag cũ
            var oldPostTags = await _context.PostTags
                .Where(pt => pt.PostId == post.Id)
                .ToListAsync();

            if (oldPostTags.Count > 0)
            {
                _context.PostTags.RemoveRange(oldPostTags);
            }

            if (string.IsNullOrWhiteSpace(tagNames))
            {
                return;
            }

            // Ví dụ:
            // "aspnet, csharp, ef-core"
            var names = tagNames
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (names.Count == 0)
            {
                return;
            }

            var tagIds = await _taxonomyService.EnsureTagsAsync(names);

            foreach (var tagId in tagIds)
            {
                _context.PostTags.Add(new PostTag
                {
                    PostId = post.Id,
                    TagId = tagId
                });
            }
        }

        // =========================================================
        // Tạo slug
        // =========================================================
        private static string GenerateSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "post";
            }

            text = text.Normalize(NormalizationForm.FormD);

            var builder = new StringBuilder();

            foreach (var character in text)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(character);

                if (category != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(character);
                }
            }

            var result = builder
                .ToString()
                .Normalize(NormalizationForm.FormC)
                .ToLowerInvariant();

            result = Regex.Replace(
                result,
                @"[^a-z0-9\s-]",
                "");

            result = Regex.Replace(
                result,
                @"\s+",
                "-");

            result = Regex.Replace(
                result,
                @"-+",
                "-");

            result = result.Trim('-');

            return string.IsNullOrWhiteSpace(result)
                ? "post"
                : result;
        }

        // =========================================================
        // Tạo slug không trùng database
        // =========================================================
        private async Task<string> GenerateUniqueSlugAsync(
            string title,
            int? currentPostId = null)
        {
            var baseSlug = GenerateSlug(title);

            var slug = baseSlug;
            var counter = 2;

            while (await _context.Posts.AnyAsync(p =>
                p.Slug == slug &&
                (!currentPostId.HasValue ||
                 p.Id != currentPostId.Value)))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            return slug;
        }
    }
}