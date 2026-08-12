using Ganss.Xss;

namespace BlogPlatform.Services
{
    public class HtmlSanitizerService : IHtmlSanitizerService
    {
        private readonly HtmlSanitizer postSanitizer;
        private readonly HtmlSanitizer commentSanitizer;

        public HtmlSanitizerService()
        {
            postSanitizer = new HtmlSanitizer();

            commentSanitizer = new HtmlSanitizer();
            commentSanitizer.AllowedTags.Clear();
            commentSanitizer.AllowedTags.Add("b");
            commentSanitizer.AllowedTags.Add("i");
            commentSanitizer.AllowedTags.Add("a");
            commentSanitizer.AllowedTags.Add("br");
            commentSanitizer.AllowedTags.Add("strong");
            commentSanitizer.AllowedTags.Add("em");

            commentSanitizer.AllowedAttributes.Clear();
            commentSanitizer.AllowedAttributes.Add("href");
            commentSanitizer.AllowedAttributes.Add("target");
            commentSanitizer.AllowedAttributes.Add("rel");
        }

        public string Sanitize(string html)
        {
            return SanitizePostContent(html);
        }

        public string SanitizePostContent(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;
            return postSanitizer.Sanitize(html);
        }

        public string SanitizeCommentContent(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;
            return commentSanitizer.Sanitize(html);
        }
    }
}
