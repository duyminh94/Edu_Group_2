using BlogPlatform.Data;
using BlogPlatform.Models.Enums;
using BlogPlatform.Services;
using BlogPlatform.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Areas.User.Controllers
{
    // UC01–UC06 — xem danh sách bài, chi tiết bài, tìm kiếm, lọc, trang cá nhân tác giả
    // Index là trang chủ của website (route mặc định)
    [Area("User")]
    private readonly ISearchService _searchService;
    public class BlogController : Controller
    {
        private readonly BlogDbContext _context;
        private readonly IAnalyticsService _analyticsService;

        public BlogController(BlogDbContext context, IAnalyticsService analyticsService, ISearchService searchService)
        {
            _context = context;
            _analyticsService = analyticsService;
            _searchService = searchService;
        }
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] SearchViewModel model)
        {
            var resultModel = await _searchService.SearchAsync(model);
            return View(resultModel);
        }

        // ===== UC01: Trang chủ - Danh sách bài viết mới nhất =====
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 10;
            var query = _context.Posts
                .AsNoTracking()
                .Where(p => p.Status == PostStatus.Published);

            int totalPosts = await query.CountAsync();

            var posts = await query
                .OrderByDescending(p => p.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

            var viewModel = new PostListViewModel
            {
                Posts = posts,
                Page = page,
                PageSize = pageSize,
                TotalPosts = totalPosts,
                PageTitle = "Bài viết mới nhất",
                Categories = await _context.Categories.AsNoTracking().ToListAsync(),
                Tags = await _context.Tags.AsNoTracking().ToListAsync()
            };

            return View(viewModel);
        }

        // ===== Lọc theo Chuyên mục =====
        public async Task<IActionResult> Category(string slug, int page = 1)
        {
            if (string.IsNullOrEmpty(slug)) return NotFound();

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Slug == slug);
            if (category == null) return NotFound();

            int pageSize = 10;
            var query = _context.Posts
                .AsNoTracking()
                .Where(p => p.CategoryId == category.Id && p.Status == PostStatus.Published);

            int totalPosts = await query.CountAsync();

            var posts = await query
                .OrderByDescending(p => p.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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
                    CategoryName = category.Name,
                    CategorySlug = category.Slug,
                    Status = p.Status,
                    PublishedAt = p.PublishedAt,
                    ViewCount = p.ViewCount,
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount
                })
                .ToListAsync();

            var viewModel = new PostListViewModel
            {
                Posts = posts,
                Page = page,
                PageSize = pageSize,
                TotalPosts = totalPosts,
                PageTitle = $"Chuyên mục: {category.Name}",
                CategorySlug = slug,
                Categories = await _context.Categories.AsNoTracking().ToListAsync(),
                Tags = await _context.Tags.AsNoTracking().ToListAsync()
            };

            return View("Index", viewModel);
        }

        // ===== Lọc theo Thẻ =====
        public async Task<IActionResult> Tag(string slug, int page = 1)
        {
            if (string.IsNullOrEmpty(slug)) return NotFound();

            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Slug == slug);
            if (tag == null) return NotFound();

            int pageSize = 10;
            var query = _context.Posts
                .AsNoTracking()
                .Where(p => p.PostTags.Any(pt => pt.TagId == tag.Id) && p.Status == PostStatus.Published);

            int totalPosts = await query.CountAsync();

            var posts = await query
                .OrderByDescending(p => p.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

            var viewModel = new PostListViewModel
            {
                Posts = posts,
                Page = page,
                PageSize = pageSize,
                TotalPosts = totalPosts,
                PageTitle = $"Thẻ: #{tag.Name}",
                TagSlug = slug,
                Categories = await _context.Categories.AsNoTracking().ToListAsync(),
                Tags = await _context.Tags.AsNoTracking().ToListAsync()
            };

            return View("Index", viewModel);
        }

        // ===== UC05: Tìm kiếm bài viết =====
        public async Task<IActionResult> Search(string? q, int page = 1)
        {
            int pageSize = 10;
            var query = _context.Posts
                .AsNoTracking()
                .Where(p => p.Status == PostStatus.Published);

            if (!string.IsNullOrWhiteSpace(q))
            {
                string keyword = q.Trim();
                query = query.Where(p => p.Title.Contains(keyword) || (p.Summary != null && p.Summary.Contains(keyword)));
            }

            int totalPosts = await query.CountAsync();

            var posts = await query
                .OrderByDescending(p => p.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

            var searchViewModel = new SearchViewModel
            {
                Query = q ?? string.Empty,
                Results = new PostListViewModel
                {
                    Posts = posts,
                    Page = page,
                    PageSize = pageSize,
                    TotalPosts = totalPosts,
                    PageTitle = string.IsNullOrWhiteSpace(q) ? "Tìm kiếm bài viết" : $"Kết quả tìm kiếm cho: \"{q}\"",
                    Categories = await _context.Categories.AsNoTracking().ToListAsync(),
                    Tags = await _context.Tags.AsNoTracking().ToListAsync()
                }
            };

            return View(searchViewModel);
        }

        // ===== UC02: Xem chi tiết bài viết =====
        public async Task<IActionResult> Detail(string slug)
        {
            if (string.IsNullOrEmpty(slug)) return NotFound();

            var post = await _context.Posts
                .AsNoTracking()
                .Include(p => p.Author)
                    .ThenInclude(a => a!.BlogSetting)
                .Include(p => p.Category)
                .Include(p => p.PostTags)
                    .ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.Status == PostStatus.Published);

            if (post == null) return NotFound();

            // Đọc cấu hình giao diện blog của tác giả
            if (post.Author?.BlogSetting != null)
            {
                ViewBag.BlogTheme = post.Author.BlogSetting.ThemeName;
                ViewBag.BlogPrimaryColor = post.Author.BlogSetting.PrimaryColor;
                ViewBag.BlogFontFamily = post.Author.BlogSetting.FontFamily;
                ViewBag.BlogLogoUrl = post.Author.BlogSetting.LogoUrl;
                ViewBag.BlogTagline = post.Author.BlogSetting.Tagline;
            }

            // Ghi nhận lượt xem qua Analytics Service
            await _analyticsService.RecordViewAsync(post.Id, HttpContext);

            var viewModel = new PostDetailViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Slug = post.Slug,
                Content = post.Content,
                Summary = post.Summary,
                FeaturedImageUrl = post.FeaturedImageUrl,
                PublishedAt = post.PublishedAt,
                AuthorId = post.AuthorId,
                AuthorDisplayName = post.Author!.DisplayName,
                AuthorUserName = post.Author.UserName,
                AuthorAvatarUrl = post.Author.AvatarUrl,
                AuthorBio = post.Author.Bio,
                CategoryName = post.Category?.Name,
                CategorySlug = post.Category?.Slug,
                Tags = post.PostTags.Select(pt => new TagViewModel
                {
                    Name = pt.Tag.Name,
                    Slug = pt.Tag.Slug
                }).ToList(),
                ViewCount = post.ViewCount,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount
            };

            return View(viewModel);
        }

        // ===== UC06: Trang tác giả =====
        public async Task<IActionResult> Author(string username, int page = 1)
        {
            if (string.IsNullOrEmpty(username)) return NotFound();

            var author = await _context.Users
                .AsNoTracking()
                .Include(u => u.BlogSetting)
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (author == null) return NotFound();

            if (author.BlogSetting != null)
            {
                ViewBag.BlogTheme = author.BlogSetting.ThemeName;
                ViewBag.BlogPrimaryColor = author.BlogSetting.PrimaryColor;
                ViewBag.BlogFontFamily = author.BlogSetting.FontFamily;
                ViewBag.BlogLogoUrl = author.BlogSetting.LogoUrl;
                ViewBag.BlogTagline = author.BlogSetting.Tagline;
            }

            int pageSize = 10;
            var query = _context.Posts
                .AsNoTracking()
                .Where(p => p.AuthorId == author.Id && p.Status == PostStatus.Published);

            int totalPosts = await query.CountAsync();
            var posts = await query
                .OrderByDescending(p => p.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostListItemViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    Summary = p.Summary,
                    FeaturedImageUrl = p.FeaturedImageUrl,
                    AuthorId = author.Id,
                    AuthorUserName = author.UserName,
                    AuthorDisplayName = author.DisplayName,
                    CategoryName = p.Category != null ? p.Category.Name : null,
                    CategorySlug = p.Category != null ? p.Category.Slug : null,
                    Status = p.Status,
                    PublishedAt = p.PublishedAt,
                    ViewCount = p.ViewCount,
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount
                })
                .ToListAsync();

            var viewModel = new AuthorProfileViewModel
            {
                AuthorId = author.Id,
                UserName = author.UserName,
                DisplayName = author.DisplayName,
                AvatarUrl = author.AvatarUrl,
                Bio = author.Bio,
                Posts = new PostListViewModel
                {
                    Posts = posts,
                    Page = page,
                    PageSize = pageSize,
                    TotalPosts = totalPosts,
                    PageTitle = $"Bài viết của {author.DisplayName}"
                }
            };

            return View("~/Areas/User/Views/Profile/Author.cshtml", viewModel);
        }

        // ===== Xử lý lỗi theo chỉ định trong TODO =====
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? code)
        {
            ViewBag.StatusCode = code ?? 500;
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}