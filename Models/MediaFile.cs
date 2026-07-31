using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Models
{
    // File ảnh/media do người dùng upload
    [Index(nameof(StoredFileName), IsUnique = true)]
    public class MediaFile
    {
        [Key]
        public int Id { get; set; }

        // Tên gốc người dùng upload — chỉ dùng để hiển thị
        [Required]
        [StringLength(255)]
        public string OriginalFileName { get; set; } = null!;

        // Tên đã đổi thành GUID khi lưu xuống ổ đĩa — chống ghi đè và chống chạy file độc
        [Required]
        [StringLength(100)]
        public string StoredFileName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string ContentType { get; set; } = null!;

        // Dung lượng file tính bằng byte, dùng để kiểm tra giới hạn
        public long SizeBytes { get; set; }

        // NULL khi file vừa upload mà chưa gắn vào bài viết nào
        public int? PostId { get; set; }

        public int UploadedById { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        // ===== Navigation =====
        [ForeignKey(nameof(PostId))]
        public Post? Post { get; set; }

        [ForeignKey(nameof(UploadedById))]
        public User? UploadedBy { get; set; }
    }
}
