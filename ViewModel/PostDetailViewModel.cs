using BlogPlatform.Models;

namespace BlogPlatform.ViewModel
{
    // Dữ liệu trang chi tiết bài viết — CONTRACT DÙNG CHUNG GIỮA 3 KHU
    //
    // ⚠️ QUY ƯỚC: chỉ Khu B (Người 2) được sửa file này.
    //    Khu C và Khu D chỉ ĐỌC. Thiếu property nào thì báo Khu B thêm, không tự sửa.
    public class PostDetailViewModel
    {
        // ===== Khu B — nội dung bài viết =====
        public Post Post { get; set; } = null!;

        // Nội dung HTML đã được sanitize, view dùng Html.Raw để render
        public string SanitizedContent { get; set; } = string.Empty;

        // Danh sách thẻ gắn cho bài, lấy sẵn để view không phải truy vấn thêm
        public List<Tag> Tags { get; set; } = new();

        // Vài bài viết liên quan hiển thị cuối trang
        public List<Post> RelatedPosts { get; set; } = new();

        // ===== Khu D — tương tác (Issue #7, #9) =====
        // Cây bình luận đã dựng sẵn, chỉ chứa bình luận đã duyệt
        public List<CommentViewModel> Comments { get; set; } = new();

        // Người đang xem đã thích / đã lưu bài này chưa
        public bool IsLiked { get; set; }
        public bool IsBookmarked { get; set; }

        // ===== Trạng thái phiên đăng nhập — mọi khu đều cần đọc =====
        public bool IsLoggedIn { get; set; }
        public int? CurrentUserId { get; set; }

        // Người đang xem có phải tác giả bài này không (để hiện nút Sửa)
        public bool IsAuthor { get; set; }
    }
}
