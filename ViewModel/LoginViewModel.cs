using System.ComponentModel.DataAnnotations;

namespace BlogPlatform.ViewModel
{
    // Form đăng nhập (Issue #3, UC08)
    // 👥 Khu A sở hữu
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [StringLength(50)]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; } = string.Empty;

        // [DataType(Password)] sinh ra input type="password" — trình duyệt tự che ký tự
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        // Trang người dùng định vào trước khi bị SessionAuthorizeAttribute đẩy về đây.
        // Đăng nhập xong redirect trở lại đúng trang đó (Issue #1, #3).
        //
        // ⚠️ Trước khi redirect phải kiểm tra Url.IsLocalUrl — nếu không, kẻ xấu gửi link
        //    kèm returnUrl trỏ sang web của họ để lừa người dùng (lỗi open redirect).
        public string? ReturnUrl { get; set; }
    }
}
