using BlogPlatform.Filters;
using BlogPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Areas.User.Controllers
{
    // UC12, UC13, UC14 — thích, lưu bài (bookmark), chia sẻ
    [Area("User")]
    [SessionAuthorize]
    public class InteractionController : Controller
    {
    }
}
