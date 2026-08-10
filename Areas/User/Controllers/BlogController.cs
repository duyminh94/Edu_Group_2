using BlogPlatform.Data;
using BlogPlatform.Models;
using BlogPlatform.Models.Enums;
using BlogPlatform.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Areas.User.Controllers
{
    // UC01–UC06
    // Xem danh sách bài, chi tiết bài, tìm kiếm,
    // lọc, trang cá nhân tác giả
    [Area("User")]
    public class BlogController : Controller
    {
        private readonly BlogDbContext _context;
        private readonly IAnalyticsService _analyticsService;

        public BlogController(BlogDbContext context, IAnalyticsService analyticsService)
        {
            _context = context;
            _analyticsService = analyticsService;
        }

        // =====================================================
        // TRANG CHỦ
        // GET: /
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Author)
                .Where(p => p.Status == PostStatus.Published)
                .OrderByDescending(p => p.PublishedAt)
                .ToListAsync();

            return View(posts);
        }

        // =====================================================
        // CHI TIẾT BÀI VIẾT
        // GET: /post/{slug}
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Detail(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.Author)
                .Include(p => p.PostTags)
                    .ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync(p =>
                    p.Slug == slug &&
                    p.Status == PostStatus.Published);

            if (post == null)
            {
                return NotFound();
            }

            // Ghi log view + tăng bộ đếm trong cùng transaction, nhưng bỏ qua tác giả xem bài của chính mình
            await _analyticsService.RecordViewAsync(post.Id, HttpContext);

            return View(post);
        }

        // =====================================================
        // TRANG TÁC GIẢ
        // GET: /author/{username}
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Author(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return NotFound();
            }

            var author = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u =>
                    u.UserName == username);

            if (author == null)
            {
                return NotFound();
            }

            var posts = await _context.Posts
                .AsNoTracking()
                .Include(p => p.Category)
                .Where(p =>
                    p.AuthorId == author.Id &&
                    p.Status == PostStatus.Published)
                .OrderByDescending(p => p.PublishedAt)
                .ToListAsync();

            ViewBag.Author = author;

            return View(posts);
        }

        // =====================================================
        // CATEGORY
        // GET: /category/{slug}
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Category(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return NotFound();
            }

            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Slug == slug);

            if (category == null)
            {
                return NotFound();
            }

            var posts = await _context.Posts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Author)
                .Where(p =>
                    p.CategoryId == category.Id &&
                    p.Status == PostStatus.Published)
                .OrderByDescending(p => p.PublishedAt)
                .ToListAsync();

            ViewBag.Category = category;

            return View(posts);
        }

        // =====================================================
        // TAG
        // GET: /tag/{slug}
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Tag(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return NotFound();
            }

            var tag = await _context.Tags
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Slug == slug);

            if (tag == null)
            {
                return NotFound();
            }

            var posts = await _context.Posts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Author)
                .Include(p => p.PostTags)
                    .ThenInclude(pt => pt.Tag)
                .Where(p =>
                    p.Status == PostStatus.Published &&
                    p.PostTags.Any(pt => pt.TagId == tag.Id))
                .OrderByDescending(p => p.PublishedAt)
                .ToListAsync();

            ViewBag.Tag = tag;

            return View(posts);
        }

        // =====================================================
        // ERROR
        // =====================================================
        [HttpGet]
        public IActionResult Error(int? code = null)
        {
            ViewBag.StatusCode = code;

            return View("~/Views/Shared/Error.cshtml");
        }
    }
}