using BlogPlatform.Data;
using BlogPlatform.Models;
using BlogPlatform.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BlogPlatform.Services
{
    public class BlogSettingService : IBlogSettingService
    {
        private readonly BlogDbContext _context;

        public static readonly List<string> AllowedThemes = new() { "light", "dark", "serif", "minimal" };
        public static readonly List<string> AllowedFonts = new() { "Be Vietnam Pro", "Roboto", "Inter", "Playfair Display" };

        public BlogSettingService(BlogDbContext context)
        {
            _context = context;
        }

        public bool IsValidHexColor(string color)
        {
            if (string.IsNullOrWhiteSpace(color)) return false;
            return Regex.IsMatch(color, "^#[0-9A-Fa-f]{6}$");
        }

        public async Task<BlogSettingViewModel> GetByUserIdAsync(int userId)
        {
            var setting = await _context.BlogSettings.FirstOrDefaultAsync(s => s.UserId == userId);

            var viewModel = new BlogSettingViewModel
            {
                AvailableThemes = AllowedThemes,
                AvailableFonts = AllowedFonts
            };

            if (setting != null)
            {
                viewModel.ThemeName = setting.ThemeName;
                viewModel.PrimaryColor = setting.PrimaryColor;
                viewModel.FontFamily = setting.FontFamily;
                viewModel.LogoUrl = setting.LogoUrl;
                viewModel.Tagline = setting.Tagline;
            }

            return viewModel;
        }

        public async Task<bool> SaveSettingAsync(int userId, BlogSettingViewModel model)
        {
            if (!IsValidHexColor(model.PrimaryColor)) return false;
            if (!AllowedFonts.Contains(model.FontFamily)) return false;

            var setting = await _context.BlogSettings.FirstOrDefaultAsync(s => s.UserId == userId);

            if (setting == null)
            {
                setting = new BlogSetting { UserId = userId };
                _context.BlogSettings.Add(setting);
            }

            setting.ThemeName = model.ThemeName;
            setting.PrimaryColor = model.PrimaryColor;
            setting.FontFamily = model.FontFamily;
            setting.LogoUrl = model.LogoUrl;
            setting.Tagline = model.Tagline;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
