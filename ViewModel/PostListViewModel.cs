using BlogPlatform.Models;

namespace BlogPlatform.ViewModel
{
    // Dữ liệu cho trang danh sách bài viết (trang chủ, theo chuyên mục, theo thẻ)
    // (Issue #4, UC01)
    // 👥 Khu B sở hữu
    //
    // Vì sao cần ViewModel bọc ngoài thay vì trả thẳng List: một View chỉ khai báo được
    // duy nhất 1 @model, mà trang này cần cùng lúc danh sách bài + thông tin phân trang
    // + tiêu đề + sidebar chuyên mục/thẻ.
    public class PostListViewModel
    {
        // Danh sách bài của trang hiện tại
        public List<PostListItemViewModel> Posts { get; set; } = new();

        // ===== Phân trang — quy tắc 6.6: 10 bài mỗi trang =====
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPosts { get; set; }

        // Tổng số trang, tính từ TotalPosts và PageSize.
        // Property chỉ đọc (chỉ có get) — tính ra từ dữ liệu đã có, không cần lưu riêng.
        public int TotalPages => (int)Math.Ceiling((double)TotalPosts / PageSize);

        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;

        // ===== Ngữ cảnh hiển thị =====

        // Tiêu đề trang: "Bài viết mới nhất" / "Chuyên mục: Lập trình" / "Thẻ: aspnet"
        public string PageTitle { get; set; } = string.Empty;

        // Slug chuyên mục hoặc thẻ đang xem — dùng dựng link phân trang cho đúng
        public string? CategorySlug { get; set; }
        public string? TagSlug { get; set; }

        // ===== Dữ liệu sidebar =====
        public List<Category> Categories { get; set; } = new();
        public List<Tag> Tags { get; set; } = new();
    }
}
