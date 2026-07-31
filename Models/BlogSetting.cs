using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogPlatform.Models
{
    // Cấu hình giao diện riêng của từng tác giả (quan hệ 1-1 với User)
    // UserId vừa là khoá chính vừa là khoá ngoại → ép quan hệ 1-1
    public class BlogSetting
    {
        [Key]
        public int UserId { get; set; }

        // Tên preset: light / dark / serif / minimal
        [Required]
        [StringLength(50)]
        public string ThemeName { get; set; } = "light";

        // Mã màu hex, đè lên màu mặc định của preset
        [Required]
        [StringLength(7)]
        public string PrimaryColor { get; set; } = "#2563eb";

        [Required]
        [StringLength(100)]
        public string FontFamily { get; set; } = "Be Vietnam Pro";

        [StringLength(500)]
        public string? LogoUrl { get; set; }

        [StringLength(200)]
        public string? Tagline { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // ===== Navigation =====
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}
