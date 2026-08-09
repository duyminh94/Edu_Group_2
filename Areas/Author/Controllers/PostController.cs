using BlogPlatform.Filters;
using BlogPlatform.Helpers;
using BlogPlatform.Services;
using BlogPlatform.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogPlatform.Areas.Author.Controllers
{
    // UC15–UC21 — tạo, sửa, xoá, publish/unpublish bài viết
    [Area("Author")]
    [SessionAuthorize(Roles = "Author,Admin")]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly ITaxonomyService _taxonomyService;

        public PostController(
            IPostService postService,
            ITaxonomyService taxonomyService)
        {
            _postService = postService;
            _taxonomyService = taxonomyService;
        }

        // Danh sách bài viết của tác giả
        // GET: /Author/Post
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var authorId = HttpContext.Session.GetInt32(SessionKeys.UserId);

            if (authorId == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new { area = "User" });
            }

            var posts = await _postService.GetByAuthorAsync(authorId.Value);

            return View(posts);
        }

        // Hiển thị form tạo bài
        // GET: /Author/Post/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new PostEditViewModel();

            await LoadCategoriesAsync(model);

            return View(model);
        }

        // Xử lý tạo bài
        // POST: /Author/Post/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(model);
                return View(model);
            }

            var authorId = HttpContext.Session.GetInt32(SessionKeys.UserId);

            if (authorId == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new { area = "User" });
            }

            await _postService.CreateAsync(model, authorId.Value);

            TempData["SuccessMessage"] = "Tạo bài viết thành công.";

            return RedirectToAction(nameof(Index));
        }

        // Hiển thị form sửa
        // GET: /Author/Post/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var authorId = HttpContext.Session.GetInt32(SessionKeys.UserId);

            if (authorId == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new { area = "User" });
            }

            var post = await _postService.GetByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            // UC21 — chỉ chủ bài viết mới được sửa
            if (post.AuthorId != authorId.Value)
            {
                return Forbid();
            }

            var model = new PostEditViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Summary = post.Summary,
                Content = post.Content,
                FeaturedImageUrl = post.FeaturedImageUrl,
                CategoryId = post.CategoryId,
                Status = post.Status,
                Slug = post.Slug,
                TagNames = string.Join(
                    ", ",
                    post.PostTags
                        .Where(x => x.Tag != null)
                        .Select(x => x.Tag!.Name))
            };

            await LoadCategoriesAsync(model);

            return View(model);
        }

        // Xử lý sửa
        // POST: /Author/Post/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PostEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(model);
                return View(model);
            }

            var authorId = HttpContext.Session.GetInt32(SessionKeys.UserId);

            if (authorId == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new { area = "User" });
            }

            var success = await _postService.UpdateAsync(
                model,
                authorId.Value);

            if (!success)
            {
                return Forbid();
            }

            TempData["SuccessMessage"] = "Cập nhật bài viết thành công.";

            return RedirectToAction(nameof(Index));
        }

        // Xóa bài viết
        // POST: /Author/Post/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var authorId = HttpContext.Session.GetInt32(SessionKeys.UserId);

            if (authorId == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new { area = "User" });
            }

            var success = await _postService.DeleteAsync(
                id,
                authorId.Value);

            if (!success)
            {
                return Forbid();
            }

            TempData["SuccessMessage"] = "Xóa bài viết thành công.";

            return RedirectToAction(nameof(Index));
        }

        // Publish bài viết
        // POST: /Author/Post/Publish/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(int id)
        {
            var authorId = HttpContext.Session.GetInt32(SessionKeys.UserId);

            if (authorId == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new { area = "User" });
            }

            var success = await _postService.PublishAsync(
                id,
                authorId.Value);

            if (!success)
            {
                return Forbid();
            }

            TempData["SuccessMessage"] = "Đăng bài thành công.";

            return RedirectToAction(nameof(Index));
        }

        // Unpublish bài viết
        // POST: /Author/Post/Unpublish/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unpublish(int id)
        {
            var authorId = HttpContext.Session.GetInt32(SessionKeys.UserId);

            if (authorId == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new { area = "User" });
            }

            var success = await _postService.UnpublishAsync(
                id,
                authorId.Value);

            if (!success)
            {
                return Forbid();
            }

            TempData["SuccessMessage"] = "Đã chuyển bài viết về bản nháp.";

            return RedirectToAction(nameof(Index));
        }

        // Đổ danh sách Category cho dropdown
        private async Task LoadCategoriesAsync(PostEditViewModel model)
        {
            var categories =
                await _taxonomyService.GetAllCategoriesAsync();

            model.Categories = new SelectList(
                categories,
                "Id",
                "Name",
                model.CategoryId);
        }
    }
}