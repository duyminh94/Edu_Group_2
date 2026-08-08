using BlogPlatform.Data;
using BlogPlatform.Models.Enums;
using BlogPlatform.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Services
{
    public class SearchService : ISearchService
    {
        private readonly BlogDbContext _context;

        public SearchService(BlogDbContext context)
        {
            _context = context;
        }

        public async Task<SearchViewModel> SearchAsync(SearchViewModel model)
        {
            var query = _context.Posts
                .AsNoTracking()
                .Where(p => p.Status == PostStatus.Published);

            // Kiểm tra xem người dùng đã tương tác tìm kiếm/lọc hay chưa
            bool isSearching = !string.IsNullOrWhiteSpace(model.Keyword)
                || !string.IsNullOrEmpty(model.CategorySlug)
                || !string.IsNullOrEmpty(model.TagSlug)
                || !string.IsNullOrEmpty(model.AuthorUserName);

            model.HasSearched = isSearching;

            // Quy tắc 6.9: Tìm theo Keyword trong Title và Summary (chỉ tìm khi >= 2 ký tự)
            if (!string.IsNullOrWhiteSpace(model.Keyword) && model.Keyword.Trim().Length >= 2)
            {
                string kw = model.Keyword.Trim();
                query = query.Where(p => p.Title.Contains(kw) || (p.Summary != null && p.Summary.Contains(kw)));
            }

            // Lọc theo Chuyên mục
            if (!string.IsNullOrEmpty(model.CategorySlug))
            {
                query = query.Where(p => p.Category != null && p.Category.Slug == model.CategorySlug);
            }

            // Lọc theo Thẻ
            if (!string.IsNullOrEmpty(model.TagSlug))
            {
                query = query.Where(p => p.PostTags.Any(pt => pt.Tag.Slug == model.TagSlug));
            }

            // Lọc theo Tác giả
            if (!string.IsNullOrEmpty(model.AuthorUserName))
            {
                query = query.Where(p => p.Author != null && p.Author.UserName == model.AuthorUserName);
            }

            // Quy tắc 6.5: Sắp xếp kết quả ("newest" / "views" / "likes")
            query = model.SortBy?.ToLower() switch
            {
                "views" => query.OrderByDescending(p => p.ViewCount),
                "likes" => query.OrderByDescending(p => p.LikeCount),
                _ => query.OrderByDescending(p => p.PublishedAt)
            };

            model.TotalCount = await query.CountAsync();

            // Projection sang PostListItemViewModel — Tuyệt đối KHÔNG lấy cột Content
            model.Results = await query
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .Select(p => new PostListItemViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    Summary = p.Summary,
                    FeaturedImageUrl = p.FeaturedImageUrl,
                    AuthorId = p.AuthorId,
                    AuthorUserName = p.Author!.UserName,
                    AuthorDisplayName = p.Author.DisplayName,
                    CategoryName = p.Category != null ? p.Category.Name : null,
                    CategorySlug = p.Category != null ? p.Category.Slug : null,
                    Status = p.Status,
                    PublishedAt = p.PublishedAt,
                    ViewCount = p.ViewCount,
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount
                })
                .ToListAsync();

            // Đổ danh sách cho Dropdown/Tags ở View
            var categories = await _context.Categories.AsNoTracking().ToListAsync();
            model.CategoryOptions = new SelectList(categories, "Slug", "Name", model.CategorySlug);
            model.AvailableTags = await _context.Tags.AsNoTracking().ToListAsync();

            return model;
        }
    }
}