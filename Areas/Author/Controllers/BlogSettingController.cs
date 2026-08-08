using BlogPlatform.Services;
using BlogPlatform.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogPlatform.Areas.Author.Controllers
{
    [Area("Author")]
    [Authorize]
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
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var model = await _blogSettingService.GetByUserIdAsync(userId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(BlogSettingViewModel model)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Gán lại dữ liệu cho Dropdown trước khi trả về View nếu ModelState invalid
            model.AvailableThemes = BlogSettingService.AllowedThemes;
            model.AvailableFonts = BlogSettingService.AllowedFonts;

            if (!ModelState.IsValid) return View(model);

            bool success = await _blogSettingService.SaveSettingAsync(userId, model);
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