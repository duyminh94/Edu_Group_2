using System.Security.Cryptography;
using System.Text;
using BlogPlatform.Data;
using BlogPlatform.Helpers;
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

        public async Task RecordViewAsync(int postId, HttpContext httpContext)
        {
            var post = await context.Posts
                .FirstOrDefaultAsync(p => p.Id == postId && p.Status == PostStatus.Published);

            if (post == null)
            {
                return;
            }

            var currentUserId = httpContext.Session.GetInt32(SessionKeys.UserId);

            if (currentUserId.HasValue && currentUserId.Value == post.AuthorId)
            {
                return;
            }

            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                ipAddress = "unknown";
            }

            var ipHash = ComputeIpHash(ipAddress);
            var threshold = DateTime.Now.AddMinutes(-30);

            var hasRecentView = await context.PostViews
                .AnyAsync(v =>
                    v.PostId == postId &&
                    v.IpHash == ipHash &&
                    v.ViewedAt >= threshold);

            if (hasRecentView)
            {
                return;
            }

            var postView = new PostView
            {
                PostId = postId,
                UserId = currentUserId,
                IpHash = ipHash,
                ViewedAt = DateTime.Now
            };

            context.PostViews.Add(postView);
            post.ViewCount += 1;

            await context.SaveChangesAsync();
        }

        public async Task<AnalyticsViewModel> GetByAuthorAsync(int authorId)
        {
            var posts = await context.Posts
                .AsNoTracking()
                .Where(p => p.AuthorId == authorId)
                .Include(p => p.Category)
                .ToListAsync();

            var postIds = posts.Select(p => p.Id).ToList();

            var viewTotals = await context.PostViews
                .AsNoTracking()
                .Where(v => postIds.Contains(v.PostId))
                .GroupBy(v => v.PostId)
                .Select(g => new
                {
                    PostId = g.Key,
                    ViewCount = g.Count()
                })
                .ToListAsync();

            var viewCountMap = viewTotals.ToDictionary(x => x.PostId, x => x.ViewCount);

            var rows = posts
                .Select(p => new PostAnalyticsRowViewModel
                {
                    PostId = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    ViewCount = p.ViewCount,
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount,
                    PublishedAt = p.PublishedAt,
                    EngagementRate = p.ViewCount == 0
                        ? 0
                        : ((double)(p.LikeCount + p.CommentCount) / p.ViewCount) * 100
                })
                .OrderByDescending(r => r.PublishedAt ?? DateTime.MinValue)
                .ToList();

            var viewRecords = await context.PostViews
                .AsNoTracking()
                .Where(v => postIds.Contains(v.PostId))
                .Select(v => new
                {
                    v.ViewedAt,
                    v.PostId
                })
                .ToListAsync();

            var viewsByDay = viewRecords
                .GroupBy(v => new DateTime(v.ViewedAt.Year, v.ViewedAt.Month, v.ViewedAt.Day))
                .Select(g => new ViewsByDayViewModel
                {
                    Date = g.Key,
                    ViewCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();

            return new AnalyticsViewModel
            {
                TotalPosts = posts.Count,
                TotalViews = posts.Sum(p => p.ViewCount),
                TotalLikes = posts.Sum(p => p.LikeCount),
                TotalComments = posts.Sum(p => p.CommentCount),
                TotalUsers = 0,
                PendingComments = 0,
                PostRows = rows,
                ViewsByDay = viewsByDay
            };
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

        private static string ComputeIpHash(string ipAddress)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(ipAddress));
            var builder = new StringBuilder();

            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
