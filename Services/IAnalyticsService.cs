using BlogPlatform.ViewModel;
using Microsoft.AspNetCore.Http;

namespace BlogPlatform.Services
{
    // Ghi nhận lượt xem (chống đếm trùng) và tổng hợp thống kê cho tác giả / admin
    public interface IAnalyticsService
    {
        Task RecordViewAsync(int postId, HttpContext httpContext);
        Task<AnalyticsViewModel> GetByAuthorAsync(int authorId);
        Task<AnalyticsViewModel> GetSystemWideAsync();
    }
}
