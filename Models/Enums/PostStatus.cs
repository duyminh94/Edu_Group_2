namespace BlogPlatform.Models.Enums
{
    // Trạng thái bài viết — lưu xuống DB dưới dạng tinyint
    public enum PostStatus
    {
        Draft = 0,
        Published = 1,
        Unpublished = 2
    }
}
