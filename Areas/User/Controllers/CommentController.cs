using BlogPlatform.Filters;
using BlogPlatform.Helpers;
using BlogPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Areas.User.Controllers
{
    // UC10, UC11 — gửi bình luận và trả lời bình luận
    [Area("User")]
    [SessionAuthorize]
    public class CommentController : Controller
    {
        private readonly ICommentService commentService;

        public CommentController(ICommentService commentService)
        {
            this.commentService = commentService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int postId, int? parentCommentId, string content, string? returnUrl)
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            if (!userId.HasValue)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để bình luận." });
                }
                return RedirectToAction("Login", "Account", new { area = "User" });
            }

            var result = await commentService.CreateAsync(postId, userId.Value, parentCommentId, content);

            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                if (result == "SUCCESS")
                {
                    return Json(new { success = true, message = "Bình luận đã được gửi thành công." });
                }
                return Json(new { success = false, message = result });
            }

            if (result != "SUCCESS")
            {
                TempData["ErrorMessage"] = result;
            }
            else
            {
                TempData["SuccessMessage"] = "Bình luận đã được gửi thành công.";
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect($"/post/{postId}");
        }
    }
}

