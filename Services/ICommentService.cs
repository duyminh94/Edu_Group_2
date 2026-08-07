using BlogPlatform.Models;
using BlogPlatform.ViewModel;

namespace BlogPlatform.Services
{
    // Nghiệp vụ bình luận: tạo, trả lời lồng nhau, kiểm duyệt (duyệt/từ chối/gắn cờ)
    public interface ICommentService
    {
        // Issue #7 — Hiển thị cây bình luận & Tạo mới bình luận
        Task<List<CommentViewModel>> GetTreeByPostAsync(int postId, int? currentUserId);
        Task<string> CreateAsync(int postId, int userId, int? parentCommentId, string content);

        // Issue #8 — Kiểm duyệt bình luận (Moderation)
        Task<List<CommentListItemViewModel>> GetPendingByAuthorAsync(int authorId);
        Task<string> ApproveAsync(int commentId, int currentUserId);
        Task<string> RejectAsync(int commentId, int currentUserId);
        Task<string> FlagAsync(int commentId, int currentUserId);
        Task<string> DeleteAsync(int commentId, int currentUserId);
        Task<int> CountPendingAsync(int authorId);
    }
}

