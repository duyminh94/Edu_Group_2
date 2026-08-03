namespace BlogPlatform.ViewModel
{
    // Số liệu thống kê: lượt xem, lượt thích, số bình luận theo từng bài và theo thời gian
    // (Issue #12, UC23 cho Author — UC28 cho Admin)
    // 👥 Khu C sở hữu
    //
    // Dùng chung cho 2 màn hình, khác nhau ở phạm vi lọc:
    //   GetByAuthorAsync   → chỉ bài của 1 tác giả
    //   GetSystemWideAsync → toàn hệ thống
    //
    // Ghi chú: CoreDay05 làm phần này bằng Task<object> + dynamic ViewBag.Stats.
    // Cố ý không làm theo — dynamic mất hết gợi ý của trình biên dịch, gõ sai tên
    // property tới lúc chạy mới biết.
    public class AnalyticsViewModel
    {
        // ===== Các con số tổng =====
        public int TotalPosts { get; set; }
        public int TotalViews { get; set; }
        public int TotalLikes { get; set; }
        public int TotalComments { get; set; }

        // Chỉ Admin dùng (UC28), trang Author để 0
        public int TotalUsers { get; set; }

        // Số bình luận đang chờ duyệt — hiện badge nhắc việc
        public int PendingComments { get; set; }

        // ===== Bảng chi tiết từng bài =====
        public List<PostAnalyticsRowViewModel> PostRows { get; set; } = new();

        // ===== Dữ liệu vẽ biểu đồ lượt xem theo ngày =====
        public List<ViewsByDayViewModel> ViewsByDay { get; set; } = new();
    }

    // Một dòng trong bảng thống kê chi tiết từng bài viết.
    // Để chung file với AnalyticsViewModel vì chỉ dùng ở đúng chỗ này.
    public class PostAnalyticsRowViewModel
    {
        public int PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }

        public DateTime? PublishedAt { get; set; }

        // Tỉ lệ tương tác = (like + comment) / view * 100, tính trong Service.
        // Bài chưa có lượt xem nào thì để 0 — tránh chia cho 0.
        public double EngagementRate { get; set; }
    }
}
