namespace BlogPlatform.Models
{
    // Model mặc định cho trang lỗi Error.cshtml
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
