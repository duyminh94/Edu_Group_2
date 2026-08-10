using BlogPlatform.Data;
using BlogPlatform.Models;
using Microsoft.AspNetCore.Http;

namespace BlogPlatform.Services
{
    public class MediaService : IMediaService
    {
        private readonly BlogDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public MediaService(
            BlogDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<string> UploadAsync(
            IFormFile file,
            int uploadedById)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Vui lòng chọn file.");
            }

            const long maxSize = 5 * 1024 * 1024;
            if (file.Length > maxSize)
            {
                throw new ArgumentException("Ảnh không được vượt quá 5MB.");
            }

            var normalizedContentType = file.ContentType
                .Split(';', StringSplitOptions.TrimEntries)
                .FirstOrDefault()
                ?? string.Empty;

            var allowedContentTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "image/jpeg",
                "image/png",
                "image/gif",
                "image/webp"
            };

            if (!allowedContentTypes.Contains(normalizedContentType))
            {
                throw new ArgumentException("Chỉ cho phép JPG, JPEG, PNG, GIF hoặc WEBP.");
            }

            var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".gif",
                ".webp"
            };

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Tên file không hợp lệ hoặc phần mở rộng không được phép.");
            }

            var safeFileName = $"{Guid.NewGuid():N}{extension}";
            var uploadFolder = Path.Combine(_environment.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot"), "uploads");
            Directory.CreateDirectory(uploadFolder);

            var filePath = Path.Combine(uploadFolder, safeFileName);

            await using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(stream);
            }

            var mediaFile = new MediaFile
            {
                OriginalFileName = Path.GetFileName(file.FileName),
                StoredFileName = safeFileName,
                ContentType = normalizedContentType,
                SizeBytes = file.Length,
                PostId = null,
                UploadedById = uploadedById,
                UploadedAt = DateTime.Now
            };

            _context.MediaFiles.Add(mediaFile);
            await _context.SaveChangesAsync();

            return "/uploads/" + safeFileName;
        }
    }
}