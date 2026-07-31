using BlogPlatform.Data;
using BlogPlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Services
{
    public class PostService : IPostService
    {
        private readonly BlogDbContext context;

        public PostService(BlogDbContext context)
        {
            this.context = context;
        }
    }
}
