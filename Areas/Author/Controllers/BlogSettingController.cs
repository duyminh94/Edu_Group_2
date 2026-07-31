using BlogPlatform.Filters;
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
    }
}
