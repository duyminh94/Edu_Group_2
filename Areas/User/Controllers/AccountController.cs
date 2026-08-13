using BlogPlatform.Filters;
using BlogPlatform.Helpers;
using BlogPlatform.Services;
using BlogPlatform.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Areas.User.Controllers
{
    // UC07, UC08, UC09 — đăng ký, đăng nhập/đăng xuất, quản lý hồ sơ cá nhân
    // Đăng nhập thành công thì lưu UserId và RoleName vào Session
    [Area("User")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(IAccountService accountService, IWebHostEnvironment webHostEnvironment)
        {
            this._accountService = accountService;
            this._webHostEnvironment = webHostEnvironment;
        }

        // Login 
        [HttpGet]

        public IActionResult Login(string? returnUrl = null)
        {
            if (HttpContext.Session.GetInt32(SessionKeys.UserId) != null)
            {
                return RedirectToAction("Index", "Blog", new { area = "User" });
            }
            var model = new LoginViewModel { ReturnUrl = returnUrl };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _accountService.ValidateLoginAsync(model.UserName, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }
            // Lưu thông tin đăng nhập vào Session
            HttpContext.Session.SetInt32(SessionKeys.UserId, user.Id);
            HttpContext.Session.SetString(SessionKeys.UserName, user.UserName);
            HttpContext.Session.SetString(SessionKeys.DisplayName, user.DisplayName);
            HttpContext.Session.SetString(SessionKeys.RoleName, user.Role.Name);
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            return RedirectToAction("Index", "Blog", new { area = "User" });
        }

        // Register
        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetInt32(SessionKeys.UserId) != null)
            {
                return RedirectToAction("Index", "Blog", new { area = "User" });
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _accountService.RegisterAsync(model);
            if (result != "User registered successfully.")
            {
                ModelState.AddModelError("", result);
                return View(model);
            }
            TempData["SuccessMessage"] = "Registration successful. Please login.";
            return RedirectToAction("Login", "Account", new { area = "User" });
        }
        [HttpGet]
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account", new { area = "User" });
        }

        // Profile
        [HttpGet]
        [SessionAuthorize]
        public async Task<IActionResult> Profile()
        { 
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "User" });
            }
            var user = await _accountService.GetByIdAsync(userId.Value);
            if (user == null)
            {
                return NotFound();

            }
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize]
        public async Task<IActionResult> Profile(string displayName, string? bio, string? avatarUrl, IFormFile? avatarFile)
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "User" });
            }
            if (string.IsNullOrWhiteSpace(displayName))
            {
                ModelState.AddModelError(string.Empty, "Display name is required.");
                var currentUser = await _accountService.GetByIdAsync(userId.Value);
                return View(currentUser);
            }

            // Xử lý file ảnh tải lên nếu có
            if (avatarFile != null && avatarFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
                var ext = Path.GetExtension(avatarFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError(string.Empty, "Chỉ chấp nhận các file ảnh có định dạng .jpg, .jpeg, .png, .gif, .webp, .svg.");
                    var currentUser = await _accountService.GetByIdAsync(userId.Value);
                    return View(currentUser);
                }

                if (avatarFile.Length > 5 * 1024 * 1024) // Giới hạn 5MB
                {
                    ModelState.AddModelError(string.Empty, "Dung lượng file ảnh không được vượt quá 5MB.");
                    var currentUser = await _accountService.GetByIdAsync(userId.Value);
                    return View(currentUser);
                }

                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "avatars");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{userId.Value}_{Guid.NewGuid():N}{ext}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }

                avatarUrl = $"/uploads/avatars/{uniqueFileName}";
            }

            var result = await _accountService.UpdateProfileAsync(userId.Value, displayName, bio, avatarUrl);
            if (result != "Profile updated successfully.")
            {
                ModelState.AddModelError("", result);
                var currentUser = await _accountService.GetByIdAsync(userId.Value);
                return View(currentUser);
            }

            // Cập nhật lại Session DisplayName để hiển thị ngay trên Navbar
            HttpContext.Session.SetString(SessionKeys.DisplayName, displayName.Trim());

            TempData["SuccessMessage"] = "Profile updated successfully.";
            var updatedUser = await _accountService.GetByIdAsync(userId.Value);
            return View(updatedUser);
        }
    }
}
