using BlogPlatform.Models.Enums;

namespace BlogPlatform.ViewModel
{
    // Một bình luận và các bình luận trả lời nó — dùng để dựng cây tối đa 3 cấp
    // Khu D (Người 4) sở hữu file này, được tự do thêm property
    public class CommentViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        // Thông tin người bình luận, lấy sẵn để view không phải truy vấn thêm
        public string DisplayName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }

        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Trạng thái bình luận
        public CommentStatus Status { get; set; } = CommentStatus.Approved;
        public bool IsPending => Status == CommentStatus.Pending;

        // Cấp hiện tại trong cây: 1 = bình luận gốc, tối đa 3
        public int Level { get; set; } = 1;

        // Các bình luận trả lời bình luận này
        public List<CommentViewModel> Replies { get; set; } = new();
    }
}

