using BlogPlatform.Models;

namespace BlogPlatform.ViewModel
{
    // Trang cá nhân tác giả /author/{username} — thông tin tác giả, theme riêng, bài viết
    // (Issue #4, UC05)
    // 👥 Khu B sở hữu
    //
    // ⚠️ Đây là 1 trong 2 trang áp theme RIÊNG của tác giả (quy tắc 5 —
    //    trang chủ và trang tìm kiếm vẫn dùng theme mặc định của hệ thống).
    public class AuthorProfileViewModel
    {
        // Thông tin tác giả của trang này
        public User Author { get; set; } = null!;

        // Cấu hình giao diện của chính tác giả đó.
        // Quy tắc 5.2 — chưa cấu hình thì Service trả về bản mặc định
        // (light, #2563eb, Be Vietnam Pro), không bao giờ trả null.
        public BlogSetting Setting { get; set; } = null!;

        // Bài viết đã publish của tác giả, có phân trang
        public List<PostListItemViewModel> Posts { get; set; } = new();

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPosts { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalPosts / PageSize);

        // ===== Số liệu tổng hiển thị dưới tên tác giả =====
        public int TotalViews { get; set; }
        public int TotalLikes { get; set; }

        // Người đang xem có phải chính chủ trang này không —
        // đúng thì hiện nút "Chỉnh sửa hồ sơ" và "Tuỳ biến giao diện"
        public bool IsOwnProfile { get; set; }
    }
}
