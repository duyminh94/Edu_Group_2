using System.ComponentModel.DataAnnotations;
using BlogPlatform.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogPlatform.ViewModel
{
    public class PostEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        [StringLength(200, ErrorMessage = "Tiêu đề tối đa 200 ký tự")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Tóm tắt tối đa 500 ký tự")]
        [Display(Name = "Tóm tắt")]
        public string? Summary { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung bài viết")]
        [Display(Name = "Nội dung")]
        public string Content { get; set; } = string.Empty;

        // URL ảnh đang được lưu trong database
        [StringLength(500)]
        [Display(Name = "Ảnh đại diện bài viết")]
        public string? FeaturedImageUrl { get; set; }

        // File ảnh mới người dùng chọn khi Create/Edit
        [Display(Name = "Ảnh đại diện bài viết")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Chuyên mục")]
        public int? CategoryId { get; set; }

        [Display(Name = "Thẻ")]
        public string? TagNames { get; set; }

        public SelectList? Categories { get; set; }

        public PostStatus Status { get; set; } = PostStatus.Draft;

        public string? Slug { get; set; }
    }
}