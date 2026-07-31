using BlogPlatform.Data;
using BlogPlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Services
{
    public class BlogSettingService : IBlogSettingService
    {
        private readonly BlogDbContext context;

        public BlogSettingService(BlogDbContext context)
        {
            this.context = context;
        }
    }
}
