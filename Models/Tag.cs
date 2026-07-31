using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Models
{
    // Thẻ gắn cho bài viết — quan hệ N-N với Post thông qua bảng PostTag
    [Index(nameof(Slug), IsUnique = true)]
    [Index(nameof(Name), IsUnique = true)]
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        // Dùng cho URL: /tag/aspnet-core
        [Required]
        [StringLength(60)]
        public string Slug { get; set; } = null!;

        // Navigation: danh sách dòng nối tới các bài viết
        public List<PostTag> PostTags { get; set; } = new();
    }
}
