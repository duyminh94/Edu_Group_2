using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Models
{
    // Chuyên mục bài viết — 1 bài viết thuộc 1 chuyên mục
    [Index(nameof(Slug), IsUnique = true)]
    [Index(nameof(Name), IsUnique = true)]
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        // Dùng cho URL thân thiện: /category/lap-trinh
        [Required]
        [StringLength(120)]
        public string Slug { get; set; } = null!;

        [StringLength(300)]
        public string? Description { get; set; }

        // Navigation: 1 chuyên mục có nhiều bài viết
        public List<Post> Posts { get; set; } = new();
    }
}
