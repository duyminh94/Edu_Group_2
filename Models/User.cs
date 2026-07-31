using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Models
{
    // Người dùng hệ thống — tự quản, không dùng ASP.NET Core Identity
    [Index(nameof(UserName), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Key]
        public int Id { get; set; }

        // Tên đăng nhập, đồng thời dùng làm URL trang cá nhân /author/{UserName}
        [Required]
        [StringLength(50)]
        public string UserName { get; set; } = null!;

        [Required]
        [StringLength(256)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        // Chỉ lưu chuỗi đã băm — tuyệt đối không lưu mật khẩu thô
        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = null!;

        // Tên hiển thị dưới bài viết và bình luận
        [Required]
        [StringLength(100)]
        public string DisplayName { get; set; } = null!;

        [StringLength(500)]
        public string? AvatarUrl { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        // Mỗi người dùng giữ đúng 1 vai trò
        public int RoleId { get; set; }

        // Admin khoá tài khoản vi phạm (UC25)
        public bool IsLocked { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ===== Navigation properties =====
        [ForeignKey(nameof(RoleId))]
        public Role? Role { get; set; }

        public BlogSetting? BlogSetting { get; set; }
        public List<Post> Posts { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
        public List<PostLike> PostLikes { get; set; } = new();
        public List<Bookmark> Bookmarks { get; set; } = new();
        public List<MediaFile> MediaFiles { get; set; } = new();
    }
}
