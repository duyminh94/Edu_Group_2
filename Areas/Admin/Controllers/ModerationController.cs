using BlogPlatform.Data;
using BlogPlatform.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Areas.Admin.Controllers
{
    // UC27 — Gỡ nội dung vi phạm trên toàn hệ thống (bài viết và bình luận) — Khu A
    [Area("Admin")]
    [SessionAuthorize(Roles = "Admin")]
    public class ModerationController : Controller
    {
        private readonly BlogDbContext _context;

        public ModerationController(BlogDbContext context)
        {
            _context = context;
        }

        // Danh sách bài viết toàn hệ thống để kiểm duyệt
        [HttpGet]
        public async Task<IActionResult> Posts()
        {
            var posts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(posts);
        }

        // Xóa bài viết vi phạm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Gỡ bài viết vi phạm thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy bài viết.";
            }
            return RedirectToAction("Posts");
        }

        // Danh sách bình luận toàn hệ thống để kiểm duyệt
        [HttpGet]
        public async Task<IActionResult> Comments()
        {
            var comments = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Post)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return View(comments);
        }

        // Xóa bình luận vi phạm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Gỡ bình luận vi phạm thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy bình luận.";
            }
            return RedirectToAction("Comments");
        }
    }
}
