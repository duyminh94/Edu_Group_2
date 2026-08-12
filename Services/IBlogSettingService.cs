using BlogPlatform.Models;
using BlogPlatform.ViewModel;

namespace BlogPlatform.Services
{
    // Đọc và cập nhật cấu hình giao diện (theme, màu, font, logo) của từng tác giả
    public interface IBlogSettingService
    {
        Task<BlogSettingViewModel> GetByUserIdAsync(int userId);
        Task<bool> SaveSettingAsync(int userId, BlogSettingViewModel model);
        bool IsValidHexColor(string color);
    }
}
