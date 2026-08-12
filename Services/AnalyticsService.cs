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

            var postRows = await context.Posts
                .AsNoTracking()
                .OrderByDescending(p => p.ViewCount)
                .Take(20)
                .Select(p => new PostAnalyticsRowViewModel
                {
                    PostId = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    ViewCount = p.ViewCount,
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount,
                    PublishedAt = p.PublishedAt,
                    EngagementRate = p.ViewCount > 0 ? Math.Round(((double)(p.LikeCount + p.CommentCount) / p.ViewCount) * 100, 1) : 0
                })
                .ToListAsync();

            return new AnalyticsViewModel
            {
                TotalPosts = totalPosts,
                TotalViews = totalViews,
                TotalLikes = totalLikes,
                TotalComments = totalComments,
                TotalUsers = totalUsers,
                PendingComments = pendingComments,
                PostRows = postRows
            };
        }

        public async Task<AnalyticsViewModel> GetByAuthorAsync(int authorId)
        {
            var authorPostsQuery = context.Posts.AsNoTracking().Where(p => p.AuthorId == authorId);
            var totalPosts = await authorPostsQuery.CountAsync();
            var totalViews = await authorPostsQuery.SumAsync(p => (int?)p.ViewCount) ?? 0;
            var totalLikes = await authorPostsQuery.SumAsync(p => (int?)p.LikeCount) ?? 0;
            var totalComments = await context.Comments.CountAsync(c => c.Post.AuthorId == authorId);
            var pendingComments = await context.Comments.CountAsync(c => c.Post.AuthorId == authorId && c.Status == CommentStatus.Pending);

            var postRows = await authorPostsQuery
                .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
                .Select(p => new PostAnalyticsRowViewModel
                {
                    PostId = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    ViewCount = p.ViewCount,
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount,
                    PublishedAt = p.PublishedAt,
                    EngagementRate = p.ViewCount > 0 ? Math.Round(((double)(p.LikeCount + p.CommentCount) / p.ViewCount) * 100, 1) : 0
                })
                .ToListAsync();

            return new AnalyticsViewModel
            {
                TotalPosts = totalPosts,
                TotalViews = totalViews,
                TotalLikes = totalLikes,
                TotalComments = totalComments,
                TotalUsers = 0,
                PendingComments = pendingComments,
                PostRows = postRows
            };
        }

        public async Task RecordViewAsync(int postId, string? ipAddress, int? userId)
        {
            var post = await context.Posts.FindAsync(postId);
            if (post != null)
            {
                post.ViewCount++;
                context.PostViews.Add(new PostView
                {
                    PostId = postId,
                    IpHash = ipAddress ?? "unknown",
                    UserId = userId,
                    ViewedAt = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
            }
        }
    }
}
