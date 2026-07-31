using BlogPlatform.Filters;
using BlogPlatform.Services;
using BlogPlatform.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Areas.Author.Controllers
{
    // UC23 — xem thống kê lượt xem, lượt thích, số bình luận của bài viết mình sở hữu
    [Area("Author")]
    [SessionAuthorize(Roles = "Author,Admin")]
    public class AnalyticsController : Controller
    {
    }
}
