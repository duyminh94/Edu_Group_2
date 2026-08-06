using BlogPlatform.Filters;
using BlogPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Areas.Admin.Controllers
{
    // UC26 — Quản lý chuyên mục (Category) và thẻ (Tag) — Khu A
    [Area("Admin")]
    [SessionAuthorize(Roles = "Admin")]
    public class TaxonomyController : Controller
    {
        private readonly ITaxonomyService _taxonomyService;

        public TaxonomyController(ITaxonomyService taxonomyService)
        {
            _taxonomyService = taxonomyService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Categories = await _taxonomyService.GetAllCategoriesAsync();
            ViewBag.Tags = await _taxonomyService.GetAllTagsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(string name, string? description)
        {
            var result = await _taxonomyService.CreateCategoryAsync(name, description);
            if (result == "SUCCESS")
            {
                TempData["SuccessMessage"] = "Tạo chuyên mục thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = result;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategory(int id, string name, string? description)
        {
            var result = await _taxonomyService.UpdateCategoryAsync(id, name, description);
            if (result == "SUCCESS")
            {
                TempData["SuccessMessage"] = "Cập nhật chuyên mục thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = result;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _taxonomyService.DeleteCategoryAsync(id);
            if (result == "SUCCESS")
            {
                TempData["SuccessMessage"] = "Xóa chuyên mục thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = result;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTag(string name)
        {
            var result = await _taxonomyService.CreateTagAsync(name);
            if (result == "SUCCESS")
            {
                TempData["SuccessMessage"] = "Tạo thẻ thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = result;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var result = await _taxonomyService.DeleteTagAsync(id);
            if (result == "SUCCESS")
            {
                TempData["SuccessMessage"] = "Xóa thẻ thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = result;
            }
            return RedirectToAction("Index");
        }
    }
}
