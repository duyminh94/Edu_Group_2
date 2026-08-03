using System.ComponentModel.DataAnnotations;
using BlogPlatform.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogPlatform.ViewModel
{
    // Form tạo và sửa bài viết — kèm dropdown chuyên mục và ô nhập thẻ
    // (Issue #5, UC15–UC19)
    // 👥 Khu C sở hữu
    public class PostEditViewModel
    {
        // 0 khi tạo mới, > 0 khi sửa. Service dựa vào đây để biết Insert hay Update.
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        [StringLength(200, ErrorMessage = "Tiêu đề tối đa 200 ký tự")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Tóm tắt tối đa 500 ký tự")]
        [Display(Name = "Tóm tắt")]
        public string? Summary { get; set; }

        // Nội dung HTML từ rich text editor.
        // ⚠️ Bắt buộc đi qua IHtmlSanitizerService trước khi lưu — chống XSS (Issue #6)
        [Required(ErrorMessage = "Vui lòng nhập nội dung bài viết")]
        [Display(Name = "Nội dung")]
        public string Content { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Ảnh đại diện bài viết")]
        public string? FeaturedImageUrl { get; set; }

        // Cho phép null vì cột CategoryId trong ERD cũng null được (quy tắc SetNull)
        [Display(Name = "Chuyên mục")]
        public int? CategoryId { get; set; }

        // Người dùng gõ thẻ dạng "aspnet, csharp, ef-core".
        // Service tách chuỗi rồi gọi ITaxonomyService.EnsureTagsAsync — thẻ chưa có thì tạo mới.
        [Display(Name = "Thẻ")]
        public string? TagNames { get; set; }

        // ===== Dữ liệu phụ đổ ra form, Controller gán trước khi return View =====

        // SelectList: kiểu chuyên dùng cho thẻ <select> qua tag helper asp-items.
        // Tạo trong Controller, không tạo trong View.
        public SelectList? Categories { get; set; }

        // ===== Chỉ để hiển thị, không cho sửa qua form =====

        // Quy tắc 1.1 — bài mới luôn Draft. Form không được phép đổi trực tiếp
        // property này, việc đổi trạng thái đi qua PublishAsync / UnpublishAsync.
        public PostStatus Status { get; set; } = PostStatus.Draft;

        // Quy tắc 1.13 — bài đã publish thì giữ nguyên slug dù có sửa tiêu đề,
        // để không hỏng link người khác đã chia sẻ. Hiện ra cho tác giả biết.
        public string? Slug { get; set; }
    }
}
