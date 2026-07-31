using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Models
{
    // Bảng trung gian nối Post và Tag
    // Khoá chính ghép (PostId, TagId) → database tự chặn gắn trùng thẻ vào cùng 1 bài
    [PrimaryKey(nameof(PostId), nameof(TagId))]
    public class PostTag
    {
        public int PostId { get; set; }
        public int TagId { get; set; }

        // ===== Navigation =====
        [ForeignKey(nameof(PostId))]
        public Post? Post { get; set; }

        [ForeignKey(nameof(TagId))]
        public Tag? Tag { get; set; }
    }
}
