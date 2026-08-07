using BlogPlatform.Filters;
using BlogPlatform.Helpers;
using BlogPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Areas.Author.Controllers
{
    // UC22 — kiểm duyệt bình luận trên bài của chính mình (duyệt / từ chối / gắn cờ)
    [Area("Author")]
    [SessionAuthorize(Roles = "Author,Admin")]
    public class CommentModerationController : Controller
    {
        private readonly ICommentService commentService;

        public CommentModerationController(ICommentService commentService)
        {
            this.commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account", new { area = "User" });
            }

            var pendingComments = await commentService.GetPendingByAuthorAsync(userId.Value);
            return View(pendingComments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập lại." });
            }

            var result = await commentService.ApproveAsync(id, userId.Value);
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (result == "SUCCESS")
            {
                if (isAjax) return Json(new { success = true, message = "Đã duyệt bình luận thành công." });
                TempData["SuccessMessage"] = "Đã duyệt bình luận thành công.";
            }
            else
            {
                if (isAjax) return Json(new { success = false, message = result });
                TempData["ErrorMessage"] = result;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập lại." });
            }

            var result = await commentService.RejectAsync(id, userId.Value);
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (result == "SUCCESS")
            {
                if (isAjax) return Json(new { success = true, message = "Đã từ chối bình luận." });
                TempData["SuccessMessage"] = "Đã từ chối bình luận.";
            }
            else
            {
                if (isAjax) return Json(new { success = false, message = result });
                TempData["ErrorMessage"] = result;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Flag(int id)
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập lại." });
            }

            var result = await commentService.FlagAsync(id, userId.Value);
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (result == "SUCCESS")
            {
                if (isAjax) return Json(new { success = true, message = "Đã gắn cờ bình luận vi phạm." });
                TempData["SuccessMessage"] = "Đã gắn cờ bình luận vi phạm.";
            }
            else
            {
                if (isAjax) return Json(new { success = false, message = result });
                TempData["ErrorMessage"] = result;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập lại." });
            }

            var result = await commentService.DeleteAsync(id, userId.Value);
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (result == "SUCCESS")
            {
                if (isAjax) return Json(new { success = true, message = "Đã xóa bình luận thành công." });
                TempData["SuccessMessage"] = "Đã xóa bình luận thành công.";
            }
            else
            {
                if (isAjax) return Json(new { success = false, message = result });
                TempData["ErrorMessage"] = result;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

