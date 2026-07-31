using BlogPlatform.Data;
using BlogPlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Services
{
    public class InteractionService : IInteractionService
    {
        private readonly BlogDbContext context;

        public InteractionService(BlogDbContext context)
        {
            this.context = context;
        }
    }
}
