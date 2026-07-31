using BlogPlatform.Filters;
using BlogPlatform.Services;
using BlogPlatform.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Areas.Author.Controllers
{
    // UC15–UC21 — tạo, sửa, xoá, publish/unpublish bài viết
    // Mọi action sửa/xoá phải kiểm tra quyền sở hữu bài (UC21) để chống lỗi IDOR
    [Area("Author")]
    [SessionAuthorize(Roles = "Author,Admin")]
    public class PostController : Controller
    {
    }
}
