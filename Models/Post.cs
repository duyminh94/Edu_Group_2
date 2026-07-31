using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlogPlatform.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Models
{
    // Bài viết — bảng trung tâm của hệ thống
    [Index(nameof(Slug), IsUnique = true)]
    [Index(nameof(Status), nameof(PublishedAt))]   // phục vụ query trang chủ: bài mới nhất đã publish
    [Index(nameof(AuthorId), nameof(Status))]      // phục vụ query trang cá nhân tác giả
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        // URL thân thiện: /post/huong-dan-aspnet
        [Required]
        [StringLength(220)]
        public string Slug { get; set; } = null!;

        // Tóm tắt hiển thị ngoài danh sách bài viết
        [StringLength(500)]
        public string? Summary { get; set; }

        // Nội dung HTML từ rich text editor — bắt buộc sanitize trước khi lưu
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Content { get; set; } = null!;

        [StringLength(500)]
        public string? FeaturedImageUrl { get; set; }

        // Cho phép NULL để không mất bài viết khi xoá chuyên mục
        public int? CategoryId { get; set; }

        // Chủ sở hữu bài viết — dùng để kiểm tra quyền khi sửa/xoá (UC21)
        public int AuthorId { get; set; }

        [Column(TypeName = "tinyint")]
        public PostStatus Status { get; set; } = PostStatus.Draft;

        // Chỉ có giá trị khi Status = Published
        public DateTime? PublishedAt { get; set; }

        // ===== Bộ đếm cache — lưu dư để đọc nhanh, tránh COUNT(*) mỗi lần render =====
        public int ViewCount { get; set; } = 0;
        public int LikeCount { get; set; } = 0;
        public int CommentCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // ===== Navigation properties =====
        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }

        [ForeignKey(nameof(AuthorId))]
        public User? Author { get; set; }

        public List<PostTag> PostTags { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
        public List<PostLike> PostLikes { get; set; } = new();
        public List<Bookmark> Bookmarks { get; set; } = new();
        public List<PostView> PostViews { get; set; } = new();
        public List<MediaFile> MediaFiles { get; set; } = new();
    }
}
