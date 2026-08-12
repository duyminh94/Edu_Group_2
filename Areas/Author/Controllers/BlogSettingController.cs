using BlogPlatform.Filters;
using BlogPlatform.Helpers;
using BlogPlatform.Services;
using BlogPlatform.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Areas.Author.Controllers
{
    // UC24 — tuỳ biến giao diện blog cá nhân (theme, màu, font, logo, tagline)
    [Area("Author")]
    [SessionAuthorize(Roles = "Author,Admin")]
    public class BlogSettingController : Controller
    {
        private readonly IBlogSettingService _blogSettingService;

        public BlogSettingController(IBlogSettingService blogSettingService)
        {
            _blogSettingService = blogSettingService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "User" });
            }

            var model = await _blogSettingService.GetByUserIdAsync(userId.Value);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(BlogSettingViewModel model)
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "User" });
            }

            model.AvailableThemes = BlogSettingService.AllowedThemes;
            model.AvailableFonts = BlogSettingService.AllowedFonts;

            if (!ModelState.IsValid) return View(model);

            bool success = await _blogSettingService.SaveSettingAsync(userId.Value, model);
            if (!success)
            {
                ModelState.AddModelError("", "Dữ liệu giao diện không hợp lệ!");
                return View(model);
            }

            TempData["SuccessMessage"] = "Lưu cấu hình giao diện thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
