using Microsoft.AspNetCore.Http;

namespace BlogPlatform.Services
{
    public interface IMediaService
    {
        Task<string> UploadAsync(
            IFormFile file,
            int uploadedById);
    }
}