using BlogPlatform.Filters;
using BlogPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Areas.Author.Controllers
{
    // UC20 — upload ảnh/media cho bài viết, trả về URL để chèn vào rich text editor
    [Area("Author")]
    [SessionAuthorize(Roles = "Author,Admin")]
    public class MediaController : Controller
    {
    }
}
