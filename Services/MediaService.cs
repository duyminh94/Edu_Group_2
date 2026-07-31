using BlogPlatform.Data;
using BlogPlatform.Models;

namespace BlogPlatform.Services
{
    public class MediaService : IMediaService
    {
        private readonly BlogDbContext context;
        private readonly IWebHostEnvironment environment;

        public MediaService(BlogDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
    }
}
