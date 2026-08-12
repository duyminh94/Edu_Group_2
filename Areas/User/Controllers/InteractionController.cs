using BlogPlatform.Filters;
using BlogPlatform.Helpers;
using BlogPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Areas.User.Controllers
{
    // UC12, UC13, UC14 — thích, lưu bài (bookmark), chia sẻ
    [Area("User")]
    [SessionAuthorize]
    public class InteractionController : Controller
    {
        private readonly IInteractionService interactionService;

        public InteractionController(IInteractionService interactionService)
        {
            this.interactionService = interactionService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLike(int postId)
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            if (!userId.HasValue)
            {
                return Json(new { isSuccess = false, message = "Vui lòng đăng nhập để thích bài viết." });
            }

            var result = await interactionService.ToggleLikeAsync(postId, userId.Value);
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleBookmark(int postId)
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            if (!userId.HasValue)
            {
                return Json(new { isSuccess = false, message = "Vui lòng đăng nhập để lưu bài viết." });
            }

            var result = await interactionService.ToggleBookmarkAsync(postId, userId.Value);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> Bookmarks()
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account", new { area = "User" });
            }

            var bookmarkedPosts = await interactionService.GetUserBookmarksAsync(userId.Value);
            return View(bookmarkedPosts);
        }
    }
}

