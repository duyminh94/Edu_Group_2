using BlogPlatform.Filters;
using BlogPlatform.Services;
using BlogPlatform.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Areas.User.Controllers
{
    // UC10, UC11 — gửi bình luận và trả lời bình luận
    [Area("User")]
    [SessionAuthorize]
    public class CommentController : Controller
    {
    }
}
