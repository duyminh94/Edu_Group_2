using BlogPlatform.Filters;
using BlogPlatform.Services;
using BlogPlatform.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Areas.Admin.Controllers
{
    // UC28 — thống kê toàn hệ thống
    [Area("Admin")]
    [SessionAuthorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IAnalyticsService analyticsService;

        public DashboardController(IAnalyticsService analyticsService)
        {
            this.analyticsService = analyticsService;
        }

        public async Task<IActionResult> Index()
        {
            var stats = await analyticsService.GetSystemWideAsync();
            return View(stats);
        }
    }
}
