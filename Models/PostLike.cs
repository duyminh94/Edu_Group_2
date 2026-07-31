using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Models
{
    // Lượt thích bài viết
    // Khoá chính ghép (PostId, UserId) → database tự chặn 1 người like 1 bài nhiều lần
    [PrimaryKey(nameof(PostId), nameof(UserId))]
    public class PostLike
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
