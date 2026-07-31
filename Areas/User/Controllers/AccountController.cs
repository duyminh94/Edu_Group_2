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
    }
}
