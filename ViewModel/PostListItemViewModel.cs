using BlogPlatform.Models.Enums;

namespace BlogPlatform.ViewModel
{
    // Một dòng bài viết trong danh sách — trang chủ, kết quả tìm kiếm, trang tác giả,
    // trang "Bài viết của tôi"
    //
    // ⚠️ KHÔNG có property Content. Quy tắc 6 cấm lấy cột Content kiểu nvarchar(max)
    //    ở màn hình danh sách. Service phải dùng Select() chiếu Post sang class này
    //    ngay trong câu SQL (projection), giống kỹ thuật ở CoreFirstDay03.
    //
    // 👥 Khu B sở hữu — Khu C dùng chung ở trang "Bài viết của tôi"
    public class PostListItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        // Dùng dựng link /post/{slug}
        public string Slug { get; set; } = string.Empty;

        // Tóm tắt hiển thị thay cho Content
        public string? Summary { get; set; }
        public string? FeaturedImageUrl { get; set; }

        // ===== Tác giả — lấy sẵn để View không phải truy vấn thêm =====
        public int AuthorId { get; set; }
        public string AuthorUserName { get; set; } = string.Empty;
        public string AuthorDisplayName { get; set; } = string.Empty;

        // ===== Chuyên mục — null khi chưa phân loại hoặc chuyên mục đã bị xoá (SetNull) =====
        public string? CategoryName { get; set; }
        public string? CategorySlug { get; set; }

        // ===== Trạng thái — Khu C cần để hiện nhãn Draft/Published =====
        public PostStatus Status { get; set; }
        public DateTime? PublishedAt { get; set; }

        // ===== Bộ đếm cache — đọc thẳng từ bảng Post, không COUNT(*) =====
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
    }
}
