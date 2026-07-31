using BlogPlatform.Data;
using BlogPlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly BlogDbContext context;

        public AnalyticsService(BlogDbContext context)
        {
            this.context = context;
        }
    }
}
