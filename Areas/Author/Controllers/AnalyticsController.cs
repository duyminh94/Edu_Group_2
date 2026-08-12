using BlogPlatform.Filters;
using BlogPlatform.Helpers;
using BlogPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Areas.Author.Controllers
{
    // UC23 — xem thống kê lượt xem, lượt thích, số bình luận của bài viết mình sở hữu
    [Area("Author")]
    [SessionAuthorize(Roles = "Author,Admin")]
    public class AnalyticsController : Controller
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var authorId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            if (authorId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "User" });
            }

            var stats = await _analyticsService.GetByAuthorAsync(authorId.Value);
            return View(stats);
        }
    }
}
