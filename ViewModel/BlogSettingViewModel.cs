using System.ComponentModel.DataAnnotations;

namespace BlogPlatform.ViewModel
{
    // Form tuỳ biến giao diện: chọn theme, màu chủ đạo, font, logo, tagline
    // (Issue #11, UC24, quy tắc 5.1–5.7)
    // 👥 Khu B sở hữu
    //
    // ⚠️ CẢNH BÁO BẢO MẬT: PrimaryColor và FontFamily được chèn thẳng vào thẻ <style>
    //    của _Layout. Không kiểm tra định dạng thì người dùng nhập
    //    "red; } body { background: url(...)" là chèn được CSS tuỳ ý.
    //    Attribute [RegularExpression] dưới đây là lớp chặn thứ nhất, Service vẫn
    //    phải gọi IsValidHexColor kiểm tra lại lần nữa ở phía server.
    public class BlogSettingViewModel
    {
        // Quy tắc 5.2 — preset: light / dark / serif / minimal
        [Required(ErrorMessage = "Vui lòng chọn giao diện")]
        [StringLength(50)]
        [Display(Name = "Giao diện")]
        public string ThemeName { get; set; } = "light";

        // Quy tắc 5.4 — bắt buộc đúng dạng hex #RRGGBB
        [Required(ErrorMessage = "Vui lòng chọn màu chủ đạo")]
        [RegularExpression("^#[0-9A-Fa-f]{6}$",
            ErrorMessage = "Màu phải đúng định dạng #RRGGBB, ví dụ #2563eb")]
        [Display(Name = "Màu chủ đạo")]
        public string PrimaryColor { get; set; } = "#2563eb";

        // Quy tắc 5.5 — chọn từ danh sách cố định, KHÔNG cho nhập tự do.
        // View render bằng <select>, Service đối chiếu lại với GetAvailableFonts().
        [Required(ErrorMessage = "Vui lòng chọn font chữ")]
        [StringLength(100)]
        [Display(Name = "Font chữ")]
        public string FontFamily { get; set; } = "Be Vietnam Pro";

        [StringLength(500)]
        [Display(Name = "Logo (đường dẫn ảnh)")]
        public string? LogoUrl { get; set; }

        [StringLength(200, ErrorMessage = "Slogan tối đa 200 ký tự")]
        [Display(Name = "Slogan")]
        public string? Tagline { get; set; }

        // ===== Dữ liệu đổ ra form, Controller gán trước khi return View =====
        public List<string> AvailableThemes { get; set; } = new();
        public List<string> AvailableFonts { get; set; } = new();
    }
}
