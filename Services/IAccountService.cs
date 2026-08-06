using BlogPlatform.Models;
using BlogPlatform.ViewModel;
namespace BlogPlatform.Services
{
    // Nghiệp vụ tài khoản: đăng ký, kiểm tra đăng nhập, đổi mật khẩu, cập nhật hồ sơ
    public interface IAccountService
    {
        Task<string> RegisterAsync(RegisterViewModel model);
        Task<User?> ValidateLoginAsync(string username, string password);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(int id);

        Task<string> UpdateProfileAsync(int userId, string displayName, string? bio, string? avatarUrl);

        Task<List<UserListItemViewModel>> GetAllAsync();

        Task<string> ChangeRoleAsync(int userId, int newRoleId, int currentAdminId);
        Task<string> ToggleLockAsync(int userId, int currentAdminId);
    }
}
