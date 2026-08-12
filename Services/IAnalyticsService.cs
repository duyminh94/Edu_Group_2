using BlogPlatform.ViewModel;

namespace BlogPlatform.Services
{
    // Ghi nhận lượt xem (chống đếm trùng) và tổng hợp thống kê cho tác giả / admin
    public interface IAnalyticsService
    {
        Task<AnalyticsViewModel> GetSystemWideAsync();
        Task<AnalyticsViewModel> GetByAuthorAsync(int authorId);
        Task RecordViewAsync(int postId, string? ipAddress, int? userId);
    }
}
