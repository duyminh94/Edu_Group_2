using BlogPlatform.Filters;
using BlogPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Areas.Admin.Controllers
{
    // UC25 — quản lý người dùng: khoá/mở tài khoản, đổi role
    [Area("Admin")]
    [SessionAuthorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
    }
}
