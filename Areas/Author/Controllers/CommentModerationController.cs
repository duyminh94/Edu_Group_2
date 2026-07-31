using BlogPlatform.Filters;
using BlogPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Areas.Author.Controllers
{
    // UC22 — kiểm duyệt bình luận trên bài của chính mình (duyệt / từ chối / gắn cờ)
    [Area("Author")]
    [SessionAuthorize(Roles = "Author,Admin")]
    public class CommentModerationController : Controller
    {
    }
}
