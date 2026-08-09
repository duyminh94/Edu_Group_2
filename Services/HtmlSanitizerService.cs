using Ganss.Xss;

namespace BlogPlatform.Services
{
    public class HtmlSanitizerService : IHtmlSanitizerService
    {
        private readonly HtmlSanitizer sanitizer;

        public HtmlSanitizerService()
        {
            sanitizer = new HtmlSanitizer();
        }

        public string Sanitize(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            return sanitizer.Sanitize(html);
        }
    }
}