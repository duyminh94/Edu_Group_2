using System.ComponentModel.DataAnnotations;

namespace BlogPlatform.ViewModel
{
    // Form đăng ký tài khoản mới (Issue #3, UC07)
    // 👥 Khu A sở hữu
    //
    // Các attribute dưới chỉ chặn lỗi định dạng. Hai quy tắc còn lại phải kiểm tra
    // trong AccountService vì cần truy vấn database:
    //   - Quy tắc 3.1: UserName và Email không được trùng người khác
    //   - Quy tắc 3.5: tài khoản mới luôn nhận role Reader
    public class RegisterViewModel
    {
        // Quy tắc 3.2 — UserName dùng luôn làm URL /author/{username} nên chỉ cho chữ
        // thường, số và dấu gạch ngang. Có dấu cách hoặc chữ hoa là link sẽ vỡ.
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập từ 3 đến 50 ký tự")]
        [RegularExpression("^[a-z0-9-]+$",
            ErrorMessage = "Tên đăng nhập chỉ gồm chữ thường, số và dấu gạch ngang")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(256)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên hiển thị")]
        [StringLength(100, ErrorMessage = "Tên hiển thị tối đa 100 ký tự")]
        [Display(Name = "Tên hiển thị")]
        public string DisplayName { get; set; } = string.Empty;

        // Quy tắc 3.3 — tối thiểu 6 ký tự, phải có cả chữ và số.
        // (?=.*[a-zA-Z]) là lookahead: "phía sau phải tồn tại ít nhất một chữ cái",
        // kiểm tra mà không tiêu thụ ký tự nào.
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        [RegularExpression("^(?=.*[a-zA-Z])(?=.*[0-9]).+$",
            ErrorMessage = "Mật khẩu phải có cả chữ và số")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        // [Compare] tự so sánh với property Password, không cần viết if trong Controller
        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
        [Compare(nameof(Password), ErrorMessage = "Mật khẩu nhập lại không khớp")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
