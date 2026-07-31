using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlogPlatform.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Models
{
    // Bình luận — hỗ trợ trả lời lồng nhau qua ParentCommentId (self-reference)
    [Index(nameof(PostId), nameof(Status))]   // phục vụ query: lấy bình luận đã duyệt của 1 bài
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        public int PostId { get; set; }

        public int UserId { get; set; }

        // NULL = bình luận gốc; có giá trị = đang trả lời bình luận khác
        // Giới hạn tối đa 3 cấp để tránh vỡ giao diện và query đệ quy nặng
        public int? ParentCommentId { get; set; }

        // Chỉ cho phép vài thẻ định dạng cơ bản, phải sanitize trước khi lưu
        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = null!;

        // Bình luận mới mặc định chờ duyệt, chưa hiển thị với người đọc
        [Column(TypeName = "tinyint")]
        public CommentStatus Status { get; set; } = CommentStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // ===== Navigation =====
        [ForeignKey(nameof(PostId))]
        public Post? Post { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        // Bình luận cha
        [ForeignKey(nameof(ParentCommentId))]
        public Comment? ParentComment { get; set; }

        // Danh sách bình luận trả lời bình luận này
        public List<Comment> Replies { get; set; } = new();
    }
}
