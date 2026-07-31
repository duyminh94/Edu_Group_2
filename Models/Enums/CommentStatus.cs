namespace BlogPlatform.Models.Enums
{
    // Trạng thái kiểm duyệt bình luận — chỉ Approved mới hiển thị với người đọc
    public enum CommentStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Flagged = 3
    }
}
