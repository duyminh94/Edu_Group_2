using BlogPlatform.Models;

namespace BlogPlatform.Services
{
    // Quản lý chuyên mục (Category) và thẻ (Tag) — Khu A sở hữu (Issue #13)
    public interface ITaxonomyService
    {
        // Lấy danh sách
        Task<List<Category>> GetAllCategoriesAsync();
        Task<List<Tag>> GetAllTagsAsync();
        Task<Category?> GetCategoryBySlugAsync(string slug);
        Task<Tag?> GetTagBySlugAsync(string slug);

        // Thao tác với Chuyên mục (Category)
        Task<string> CreateCategoryAsync(string name, string? description);
        Task<string> UpdateCategoryAsync(int id, string name, string? description);
        Task<string> DeleteCategoryAsync(int id);

        // Thao tác với Thẻ (Tag)
        Task<string> CreateTagAsync(string name);
        Task<string> DeleteTagAsync(int id);

        // Tự động tạo thẻ mới nếu chưa tồn tại và trả về danh sách TagId
        Task<List<int>> EnsureTagsAsync(List<string> tagNames);
    }
}
