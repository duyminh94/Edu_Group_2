using BlogPlatform.Data;
using BlogPlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Services
{
    public class CommentService : ICommentService
    {
        private readonly BlogDbContext context;

        public CommentService(BlogDbContext context)
        {
            this.context = context;
        }
    }
}
