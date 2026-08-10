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
        private readonly IMediaService _mediaService;

        public PostController(
            IPostService postService,
            ITaxonomyService taxonomyService,
            IMediaService mediaService)
        {
            _postService = postService;
            _taxonomyService = taxonomyService;
            _mediaService = mediaService;
        }

        // =====================================================
        // DANH SÁCH BÀI VIẾT
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Console.WriteLine("======================================");
            Console.WriteLine("AUTHOR POST INDEX START");

            var authorId =
                HttpContext.Session.GetInt32(SessionKeys.UserId);

            Console.WriteLine($"AUTHOR ID = {authorId}");

            if (authorId == null)
            {
                Console.WriteLine("AUTHOR ID NULL");

                return RedirectToAction(
                    "Login",
                    "Account",
                    new { area = "User" });
            }

            var posts =
                await _postService.GetByAuthorAsync(authorId.Value);

            Console.WriteLine($"POST COUNT = {posts.Count}");

            Console.WriteLine("AUTHOR POST INDEX END");
            Console.WriteLine("======================================");

            return View(posts);
        }

        // =====================================================
        // HIỂN THỊ FORM TẠO BÀI
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new PostEditViewModel();

            await LoadCategoriesAsync(model);

            return View(model);
        }

        // =====================================================
        // XỬ LÝ TẠO BÀI
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            PostEditViewModel model)
        {
            var authorId =
                HttpContext.Session.GetInt32(SessionKeys.UserId);

            if (authorId == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new { area = "User" });
            }

            // Upload ảnh nếu có
            if (model.ImageFile != null &&
                model.ImageFile.Length > 0)
            {
                try
                {
                    var imageUrl =
                        await _mediaService.UploadAsync(
                            model.ImageFile,
                            authorId.Value);

                    model.FeaturedImageUrl = imageUrl;
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(
                        "ImageFile",
                        ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(
                        "ImageFile",
                        "Không thể upload ảnh.");
                }
            }

            // Model không hợp lệ
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(model);

                return View(model);
            }

            // Tạo bài viết
            await _postService.CreateAsync(
                model,
                authorId.Value);

            TempData["SuccessMessage"] =
                "Tạo bài viết thành công.";

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // HIỂN THỊ FORM SỬA
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var authorId =
                HttpContext.Session.GetInt32(SessionKeys.UserId);

            if (authorId == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new { area = "User" });
            }

            var post =
                await _postService.GetByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            // Chỉ chủ bài viết mới được sửa
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

        // =====================================================
        // XỬ LÝ SỬA
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            PostEditViewModel model)
        {
            var authorId =
                HttpContext.Session.GetInt32(SessionKeys.UserId);

            if (authorId == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new { area = "User" });
            }

            var post =
                await _postService.GetByIdAsync(model.Id);

            if (post == null)
            {
                return NotFound();
            }

            if (post.AuthorId != authorId.Value)
            {
                return Forbid();
            }

            // Giữ ảnh cũ
            model.FeaturedImageUrl =
                post.FeaturedImageUrl;

            // Nếu chọn ảnh mới
            if (model.ImageFile != null &&
                model.ImageFile.Length > 0)
            {
                try
                {
                    var imageUrl =
                        await _mediaService.UploadAsync(
                            model.ImageFile,
                            authorId.Value);

                    model.FeaturedImageUrl = imageUrl;
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(
                        "ImageFile",
                        ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(
                        "ImageFile",
                        "Không thể upload ảnh.");
                }
            }

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(model);

                return View(model);
            }

            var success =
                await _postService.UpdateAsync(
                    model,
                    authorId.Value);

            if (!success)
            {
                return Forbid();
            }

            TempData["SuccessMessage"] =
                "Cập nhật bài viết thành công.";

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // XÓA BÀI
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var authorId =
                HttpContext.Session.GetInt32(SessionKeys.UserId);

            if (authorId == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new { area = "User" });
            }

            var success =
                await _postService.DeleteAsync(
                    id,
                    authorId.Value);

            if (!success)
            {
                return Forbid();
            }

            TempData["SuccessMessage"] =
                "Xóa bài viết thành công.";

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // PUBLISH
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(int id)
        {
            var authorId =
                HttpContext.Session.GetInt32(SessionKeys.UserId);

            if (authorId == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new { area = "User" });
            }

            var success =
                await _postService.PublishAsync(
                    id,
                    authorId.Value);

            if (!success)
            {
                return Forbid();
            }

            TempData["SuccessMessage"] =
                "Đăng bài thành công.";

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // UNPUBLISH
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unpublish(int id)
        {
            var authorId =
                HttpContext.Session.GetInt32(SessionKeys.UserId);

            if (authorId == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new { area = "User" });
            }

            var success =
                await _postService.UnpublishAsync(
                    id,
                    authorId.Value);

            if (!success)
            {
                return Forbid();
            }

            TempData["SuccessMessage"] =
                "Đã chuyển bài viết về bản nháp.";

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // LOAD CATEGORY
        // =====================================================
        private async Task LoadCategoriesAsync(
            PostEditViewModel model)
        {
            var categories =
                await _taxonomyService
                    .GetAllCategoriesAsync();

            model.Categories =
                new SelectList(
                    categories,
                    "Id",
                    "Name",
                    model.CategoryId);
        }
    }
}