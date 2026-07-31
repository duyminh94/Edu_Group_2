using BlogPlatform.Services;
using BlogPlatform.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Areas.User.Controllers
{
    // UC01–UC06 — xem danh sách bài, chi tiết bài, tìm kiếm, lọc, trang cá nhân tác giả
    // Index là trang chủ của website (route mặc định)
    // TODO: thêm action Error() trả về view Shared/Error.cshtml — UseExceptionHandler đang trỏ tới /User/Blog/Error
    [Area("User")]
    public class BlogController : Controller
    {
    }
}
