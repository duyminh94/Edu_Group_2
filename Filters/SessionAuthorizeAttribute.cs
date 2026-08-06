using BlogPlatform.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlogPlatform.Filters
{
    // Chặn request khi Session chưa có UserId, hoặc role trong Session không nằm trong danh sách cho phép
    // Dùng thay cho [Authorize] vì dự án không dùng ASP.NET Core Identity
    // Cách dùng: [SessionAuthorize] hoặc [SessionAuthorize(Roles = "Author,Admin")]
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        // Danh sách role được phép, cách nhau bởi dấu phẩy. Để trống nghĩa là chỉ cần đăng nhập
        public string Roles { get; set; } = string.Empty;

        // TODO: override OnActionExecuting — đọc Session rồi xử lý theo 3 trường hợp:
        //   1. Session chưa có UserId  → redirect về /User/Account/Login,
        //      kèm returnUrl để đăng nhập xong quay lại đúng trang đang xem
        //   2. Đã đăng nhập nhưng role không nằm trong Roles → trả về 403 Forbidden
        //   3. Hợp lệ → cho request đi tiếp
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userId = session.GetInt32(SessionKeys.UserId);
            var roleName = session.GetString(SessionKeys.RoleName);

            //  1. Chưa đăng nhập -> Chuyển hướng về trang Login kèm returnUrl

            if(userId == null)
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("Login", "Account", new { areas  = "Users" , returnUrl });
                return;
            }

            // 2. Đã đăng nhập -> Kiểm tra Role nếu thuộc tính Roles có khai báo
            if (!string.IsNullOrEmpty(Roles))
            {
                var allowedRoles = Roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if(string.IsNullOrEmpty(roleName) || !allowedRoles.Contains(roleName , StringComparer.OrdinalIgnoreCase))
                {
                    context.Result = new StatusCodeResult(403);
                    return;
                }

                base.OnActionExecuting(context);
            }
   
        }

    }
}
