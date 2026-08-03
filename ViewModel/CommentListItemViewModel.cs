using BlogPlatform.Models.Enums;

namespace BlogPlatform.ViewModel
{
    // Một dòng bình luận trong hàng chờ kiểm duyệt (Issue #8, UC22)
    //
    // PHÂN BIỆT với CommentViewModel:
    //   CommentViewModel         = bình luận cho người đọc, có cây trả lời (Replies)
    //   CommentListItemViewModel = bình luận nhìn từ góc người duyệt, phẳng, kèm tên bài
    //
    // 👥 Khu D sở hữu — Khu A đọc lại ở trang Admin/Moderation
    public class CommentListItemViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public CommentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // ===== Người bình luận =====
        public int UserId { get; set; }
        public string UserDisplayName { get; set; } = string.Empty;

        // ===== Bài viết chứa bình luận — người duyệt cần biết đang duyệt trên bài nào =====
        public int PostId { get; set; }
        public string PostTitle { get; set; } = string.Empty;
        public string PostSlug { get; set; } = string.Empty;

        // Quy tắc 2.9 — chỉ chủ bài viết hoặc Admin mới được duyệt
        public int PostAuthorId { get; set; }

        // Trích nội dung bình luận cha, để người duyệt hiểu ngữ cảnh khi đây là câu trả lời
        public string? ParentExcerpt { get; set; }
    }
}
