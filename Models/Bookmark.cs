using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Models
{
    // Bài viết được lưu lại để đọc sau
    // Khoá chính ghép (PostId, UserId) giống PostLike nhưng tách bảng riêng vì là 2 hành vi khác nhau
    [PrimaryKey(nameof(PostId), nameof(UserId))]
    public class Bookmark
    {
        public int PostId { get; set; }
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ===== Navigation =====
        [ForeignKey(nameof(PostId))]
        public Post? Post { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}
