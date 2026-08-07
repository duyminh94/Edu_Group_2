using BlogPlatform.ViewModel;

namespace BlogPlatform.Services
{
    // Tương tác của người đọc: thích, bỏ thích, lưu bài (bookmark)
    public interface IInteractionService
    {
        Task<ToggleResultViewModel> ToggleLikeAsync(int postId, int userId);
        Task<ToggleResultViewModel> ToggleBookmarkAsync(int postId, int userId);
        Task<bool> IsLikedAsync(int postId, int userId);
        Task<bool> IsBookmarkedAsync(int postId, int userId);
        Task<List<PostListItemViewModel>> GetUserBookmarksAsync(int userId);
    }
}

