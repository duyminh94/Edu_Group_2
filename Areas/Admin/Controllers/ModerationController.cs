using BlogPlatform.Filters;
using BlogPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Areas.Admin.Controllers
{
    // UC27 — gỡ nội dung vi phạm trên toàn hệ thống (bài viết và bình luận)
    [Area("Admin")]
    [SessionAuthorize(Roles = "Admin")]
    public class ModerationController : Controller
    {
    }
}
