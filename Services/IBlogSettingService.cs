using BlogPlatform.ViewModel;

namespace BlogPlatform.Services
{
    public interface IBlogSettingService
    {
        Task<BlogSettingViewModel> GetByUserIdAsync(int userId);
        Task<bool> SaveSettingAsync(int userId, BlogSettingViewModel model);
        bool IsValidHexColor(string color);
    }
}