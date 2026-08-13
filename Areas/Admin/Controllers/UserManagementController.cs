using BlogPlatform.Filters;
using BlogPlatform.Helpers;
using BlogPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Areas.Admin.Controllers
{
    // UC25 — quản lý người dùng: khoá/mở tài khoản, đổi role
    [Area("Admin")]
    [SessionAuthorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly IAccountService _accountService;

        public UserManagementController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _accountService.GetAllAsync();
            return View(users);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(int userId, int newRoleId)
        {
            var currentAdminId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            if (currentAdminId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Admin" });
            }
            var result = await _accountService.ChangeRoleAsync(userId, newRoleId, currentAdminId.Value);
            if (result.Contains("Successfully") || result == "Success" || result == "SUCCESS")
            {
                TempData["SuccessMessage"] = "Đổi vai trò người dùng thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = result;
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLock(int userId)
        {
            var currentAdminId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            if (currentAdminId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Admin" });
            }
            var result = await _accountService.ToggleLockAsync(userId, currentAdminId.Value);
            if (result.Contains("locked") || result.Contains("unlocked") || result.StartsWith("User Account") || result == "SUCCESS")
            {
                TempData["SuccessMessage"] = result.Contains("unlocked") ? "Đã mở khóa tài khoản thành công!" : "Đã khóa tài khoản thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = result;
            }
            return RedirectToAction("Index");
        }
    }
}
