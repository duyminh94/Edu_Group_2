using BlogPlatform.Models;
using BlogPlatform.ViewModel;

namespace BlogPlatform.Services
{
    // Nghiệp vụ bài viết: tạo, sửa, xoá, publish/unpublish, sinh slug
    public interface IPostService
    {
        // Danh sách bài viết của tác giả
        Task<List<Post>> GetByAuthorAsync(int authorId);

        // Lấy bài viết theo Id
        Task<Post?> GetByIdAsync(int id);

        // Tạo bài viết mới
        Task<Post> CreateAsync(PostEditViewModel model, int authorId);

        // Cập nhật bài viết
        Task<bool> UpdateAsync(PostEditViewModel model, int authorId);

        // Xóa bài viết
        Task<bool> DeleteAsync(int id, int authorId);

        // Publish bài viết
        Task<bool> PublishAsync(int id, int authorId);

        // Unpublish bài viết
        Task<bool> UnpublishAsync(int id, int authorId);
    }
}