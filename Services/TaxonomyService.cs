using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using BlogPlatform.Data;
using BlogPlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Services
{
    // Triển khai quản lý chuyên mục và thẻ — Khu A sở hữu (Issue #13)
    public class TaxonomyService : ITaxonomyService
    {
        private readonly BlogDbContext _context;

        public TaxonomyService(BlogDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả chuyên mục
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        // Lấy tất cả thẻ
        public async Task<List<Tag>> GetAllTagsAsync()
        {
            return await _context.Tags
                .AsNoTracking()
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        // Lấy chuyên mục theo slug
        public async Task<Category?> GetCategoryBySlugAsync(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return null;
            return await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Slug.ToLower() == slug.Trim().ToLower());
        }

        // Lấy thẻ theo slug
        public async Task<Tag?> GetTagBySlugAsync(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return null;
            return await _context.Tags
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Slug.ToLower() == slug.Trim().ToLower());
        }

        // Tạo chuyên mục mới
        public async Task<string> CreateCategoryAsync(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "Tên chuyên mục không được để trống.";
            }

            var trimmedName = name.Trim();
            var exists = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == trimmedName.ToLower());
            if (exists)
            {
                return "Chuyên mục với tên này đã tồn tại.";
            }

            var slug = GenerateSlug(trimmedName);
            var category = new Category
            {
                Name = trimmedName,
                Description = description?.Trim(),
                Slug = slug
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return "SUCCESS";
        }

        // Cập nhật chuyên mục
        public async Task<string> UpdateCategoryAsync(int id, string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "Tên chuyên mục không được để trống.";
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return "Không tìm thấy chuyên mục.";
            }

            var trimmedName = name.Trim();
            var duplicate = await _context.Categories
                .AnyAsync(c => c.Id != id && c.Name.ToLower() == trimmedName.ToLower());
            if (duplicate)
            {
                return "Tên chuyên mục đã được sử dụng bởi chuyên mục khác.";
            }

            category.Name = trimmedName;
            category.Description = description?.Trim();
            category.Slug = GenerateSlug(trimmedName);

            await _context.SaveChangesAsync();
            return "SUCCESS";
        }

        // Xóa chuyên mục (EF Core SetNull tự động giữ bài viết)
        public async Task<string> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return "Không tìm thấy chuyên mục.";
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return "SUCCESS";
        }

        // Tạo thẻ mới
        public async Task<string> CreateTagAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "Tên thẻ không được để trống.";
            }

            var trimmedName = name.Trim();
            var exists = await _context.Tags
                .AnyAsync(t => t.Name.ToLower() == trimmedName.ToLower());
            if (exists)
            {
                return "Thẻ với tên này đã tồn tại.";
            }

            var tag = new Tag
            {
                Name = trimmedName,
                Slug = GenerateSlug(trimmedName)
            };

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return "SUCCESS";
        }

        // Xóa thẻ (Cascades xóa PostTag)
        public async Task<string> DeleteTagAsync(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return "Không tìm thấy thẻ.";
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return "SUCCESS";
        }

        // Gắn thẻ khi lưu bài — tự động tạo thẻ nếu chưa tồn tại và trả danh sách TagId
        public async Task<List<int>> EnsureTagsAsync(List<string> tagNames)
        {
            var tagIds = new List<int>();
            if (tagNames == null || !tagNames.Any())
            {
                return tagIds;
            }

            foreach (var rawName in tagNames)
            {
                if (string.IsNullOrWhiteSpace(rawName)) continue;
                var trimmedName = rawName.Trim();
                var tag = await _context.Tags
                    .FirstOrDefaultAsync(t => t.Name.ToLower() == trimmedName.ToLower());

                if (tag == null)
                {
                    tag = new Tag
                    {
                        Name = trimmedName,
                        Slug = GenerateSlug(trimmedName)
                    };
                    _context.Tags.Add(tag);
                    await _context.SaveChangesAsync();
                }

                if (!tagIds.Contains(tag.Id))
                {
                    tagIds.Add(tag.Id);
                }
            }

            return tagIds;
        }

        // Hàm tạo slug hỗ trợ tiếng Việt
        private static string GenerateSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            text = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var ch in text)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            var cleanStr = sb.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
            cleanStr = Regex.Replace(cleanStr, @"[^a-z0-9\s-]", "");
            cleanStr = Regex.Replace(cleanStr, @"\s+", "-").Trim('-');
            return cleanStr;
        }
    }
}
