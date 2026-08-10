using Ganss.Xss;

namespace BlogPlatform.Services
{
    public class HtmlSanitizerService : IHtmlSanitizerService
    {
        private readonly HtmlSanitizer sanitizer;

        public HtmlSanitizerService()
        {
            sanitizer = new HtmlSanitizer();

            sanitizer.AllowedTags.Clear();
            sanitizer.AllowedTags.Add("p");
            sanitizer.AllowedTags.Add("br");
            sanitizer.AllowedTags.Add("strong");
            sanitizer.AllowedTags.Add("b");
            sanitizer.AllowedTags.Add("em");
            sanitizer.AllowedTags.Add("i");
            sanitizer.AllowedTags.Add("u");
            sanitizer.AllowedTags.Add("s");
            sanitizer.AllowedTags.Add("sub");
            sanitizer.AllowedTags.Add("sup");
            sanitizer.AllowedTags.Add("blockquote");
            sanitizer.AllowedTags.Add("code");
            sanitizer.AllowedTags.Add("pre");
            sanitizer.AllowedTags.Add("ul");
            sanitizer.AllowedTags.Add("ol");
            sanitizer.AllowedTags.Add("li");
            sanitizer.AllowedTags.Add("a");
            sanitizer.AllowedTags.Add("img");
            sanitizer.AllowedTags.Add("span");
            sanitizer.AllowedTags.Add("div");

            sanitizer.AllowedAttributes.Clear();
            sanitizer.AllowedAttributes.Add("href");
            sanitizer.AllowedAttributes.Add("title");
            sanitizer.AllowedAttributes.Add("target");
            sanitizer.AllowedAttributes.Add("rel");
            sanitizer.AllowedAttributes.Add("src");
            sanitizer.AllowedAttributes.Add("alt");
            sanitizer.AllowedAttributes.Add("class");
            sanitizer.AllowedAttributes.Add("style");

            sanitizer.AllowedCssProperties.Clear();
            sanitizer.AllowedCssProperties.Add("color");
            sanitizer.AllowedCssProperties.Add("font-weight");
            sanitizer.AllowedCssProperties.Add("font-style");
            sanitizer.AllowedCssProperties.Add("text-decoration");
            sanitizer.AllowedCssProperties.Add("text-align");
            sanitizer.AllowedCssProperties.Add("background-color");
            sanitizer.AllowedCssProperties.Add("margin-left");
            sanitizer.AllowedCssProperties.Add("margin-right");

            sanitizer.AllowedSchemes.Clear();
            sanitizer.AllowedSchemes.Add("http");
            sanitizer.AllowedSchemes.Add("https");
            sanitizer.AllowedSchemes.Add("mailto");
            sanitizer.AllowedSchemes.Add("data");
        }

        public string Sanitize(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            var sanitized = sanitizer.Sanitize(html);
            return sanitized ?? string.Empty;
        }
    }
}