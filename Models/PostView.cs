using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Models
{
    // Log lượt xem phục vụ thống kê theo thời gian
    [Index(nameof(PostId), nameof(ViewedAt))]
    public class PostView
    {
        // Dùng long vì đây là bảng tăng nhanh nhất hệ thống
        [Key]
        public long Id { get; set; }

        public int PostId { get; set; }

        // NULL khi người xem là khách chưa đăng nhập
        public int? UserId { get; set; }

        // Băm SHA-256 của địa chỉ IP — vẫn chặn được đếm trùng mà không lưu dữ liệu cá nhân
        [Required]
        [StringLength(64)]
        public string IpHash { get; set; } = null!;

        public DateTime ViewedAt { get; set; } = DateTime.Now;

        // ===== Navigation =====
        [ForeignKey(nameof(PostId))]
        public Post? Post { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}
