using BlogPlatform.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlogPlatform.Filters
{
    // Kiểm tra đăng nhập và quyền dựa trên Session
    //
    // Cách dùng:
    // [SessionAuthorize]
    //
    // hoặc:
    // [SessionAuthorize(Roles = "Author,Admin")]
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        // Danh sách role được phép, cách nhau bằng dấu phẩy.
        // Ví dụ: "Author,Admin"
        public string Roles { get; set; } = string.Empty;

        public override void OnActionExecuting(
            ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;

            var userId =
                session.GetInt32(SessionKeys.UserId);

            var roleName =
                session.GetString(SessionKeys.RoleName);

            // DEBUG
            Console.WriteLine(
                "========================================");

            Console.WriteLine(
                "SessionAuthorize");

            Console.WriteLine(
                $"Path: {context.HttpContext.Request.Path}");

            Console.WriteLine(
                $"UserId: {userId}");

            Console.WriteLine(
                $"RoleName: [{roleName}]");

            Console.WriteLine(
                $"Required Roles: [{Roles}]");

            Console.WriteLine(
                "========================================");

            // =====================================================
            // 1. CHƯA ĐĂNG NHẬP
            // =====================================================

            if (userId == null)
            {
                Console.WriteLine(
                    "SessionAuthorize: CHUA DANG NHAP");

                var returnUrl =
                    context.HttpContext.Request.Path +
                    context.HttpContext.Request.QueryString;

                context.Result =
                    new RedirectToActionResult(
                        "Login",
                        "Account",
                        new
                        {
                            area = "User",
                            returnUrl = returnUrl
                        });

                return;
            }

            // =====================================================
            // 2. KIỂM TRA ROLE
            // =====================================================

            if (!string.IsNullOrWhiteSpace(Roles))
            {
                var allowedRoles =
                    Roles.Split(
                        ',',
                        StringSplitOptions.RemoveEmptyEntries |
                        StringSplitOptions.TrimEntries);

                var hasPermission =
                    !string.IsNullOrWhiteSpace(roleName) &&
                    allowedRoles.Contains(
                        roleName,
                        StringComparer.OrdinalIgnoreCase);

                if (!hasPermission)
                {
                    Console.WriteLine(
                        "SessionAuthorize: KHONG CO QUYEN");

                    Console.WriteLine(
                        $"Role hien tai: [{roleName}]");

                    Console.WriteLine(
                        $"Role cho phep: [{Roles}]");

                    // Không để trang trắng.
                    context.Result =
                        new ContentResult
                        {
                            StatusCode = 403,
                            ContentType = "text/html; charset=utf-8",
                            Content =
                                "<!DOCTYPE html>" +
                                "<html lang='vi'>" +
                                "<head>" +
                                "<meta charset='utf-8'>" +
                                "<title>403 - Không có quyền</title>" +
                                "<style>" +
                                "body{font-family:Arial;margin:50px;}" +
                                "a{display:inline-block;margin-top:20px;}" +
                                "</style>" +
                                "</head>" +
                                "<body>" +
                                "<h1>403 - Không có quyền truy cập</h1>" +
                                "<p>Tài khoản hiện tại không có quyền truy cập trang này.</p>" +
                                $"<p>Role hiện tại: <strong>{roleName ?? "(null)"}</strong></p>" +
                                $"<p>Role yêu cầu: <strong>{Roles}</strong></p>" +
                                "<a href='/'>Quay về trang chủ</a>" +
                                "</body>" +
                                "</html>"
                        };

                    return;
                }
            }

            // =====================================================
            // 3. HỢP LỆ
            // =====================================================

            Console.WriteLine(
                "SessionAuthorize: OK - CHO PHEP REQUEST");

            base.OnActionExecuting(context);
        }
    }
}