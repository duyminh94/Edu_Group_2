namespace BlogPlatform.Services
{
    // Làm sạch HTML từ rich text editor trước khi lưu DB — chống XSS
    public interface IHtmlSanitizerService
    {
        string Sanitize(string html);
        string SanitizePostContent(string html);
        string SanitizeCommentContent(string html);
    }
}
