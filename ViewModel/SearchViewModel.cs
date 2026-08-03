using BlogPlatform.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogPlatform.ViewModel
{
    // Từ khoá, bộ lọc, tiêu chí sắp xếp và kết quả tìm kiếm (Issue #10, UC03, UC04)
    // 👥 Khu B sở hữu
    //
    // Class này vừa là ĐẦU VÀO vừa là ĐẦU RA của ISearchService.SearchAsync:
    // Controller nhận nó từ query string (model binding), truyền xuống Service,
    // Service điền thêm Results rồi trả về chính nó.
    //
    // Nhờ vậy sau khi tìm, ô Search và các dropdown vẫn giữ nguyên giá trị vừa chọn
    // (quy tắc 6.7) — đúng cách StudentViewModel giữ lại Filter ở CoreFirstDay03.
    public class SearchViewModel
    {
        // ===== Điều kiện lọc — người dùng nhập, khớp tên với query string =====
        // /User/Blog/Search?Keyword=aspnet&CategorySlug=lap-trinh&SortBy=views

        // Quy tắc 6.8 — để trống thì trả danh sách mới nhất, không báo lỗi
        // Quy tắc 6.9 — dưới 2 ký tự thì không tìm, tránh quét toàn bảng
        public string? Keyword { get; set; }

        public string? CategorySlug { get; set; }
        public string? TagSlug { get; set; }
        public string? AuthorUserName { get; set; }

        // Quy tắc 6.5 — nhận "newest" (mặc định) / "views" / "likes"
        public string SortBy { get; set; } = "newest";

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // ===== Kết quả — Service điền vào, người dùng không nhập =====
        public List<PostListItemViewModel> Results { get; set; } = new();

        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        // Đã bấm tìm hay chưa — dùng để phân biệt "mới vào trang" với
        // "đã tìm nhưng không ra kết quả" (quy tắc 6.10)
        public bool HasSearched { get; set; }

        // ===== Dữ liệu đổ dropdown bộ lọc, Controller gán trước khi return View =====
        public SelectList? CategoryOptions { get; set; }
        public List<Tag> AvailableTags { get; set; } = new();
    }
}
