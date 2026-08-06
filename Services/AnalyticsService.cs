using BlogPlatform.Data;
using BlogPlatform.Models;
using BlogPlatform.Models.Enums;
using BlogPlatform.ViewModel;
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

        public async Task<AnalyticsViewModel> GetSystemWideAsync()
        {
            var totalPosts = await context.Posts.CountAsync();
            var totalViews = await context.Posts.SumAsync(p => (int?)p.ViewCount) ?? 0;
            var totalLikes = await context.Posts.SumAsync(p => (int?)p.LikeCount) ?? 0;
            var totalComments = await context.Comments.CountAsync();
            var totalUsers = await context.Users.CountAsync();
            var pendingComments = await context.Comments.CountAsync(c => c.Status == CommentStatus.Pending);

            return new AnalyticsViewModel
            {
                TotalPosts = totalPosts,
                TotalViews = totalViews,
                TotalLikes = totalLikes,
                TotalComments = totalComments,
                TotalUsers = totalUsers,
                PendingComments = pendingComments
            };
        }
    }
}
