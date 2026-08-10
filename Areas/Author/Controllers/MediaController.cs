using BlogPlatform.Filters;
using BlogPlatform.Helpers;
using BlogPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Areas.Author.Controllers
{
    [Area("Author")]
    [SessionAuthorize(Roles = "Author,Admin")]
    public class MediaController : Controller
    {
        private readonly IMediaService _mediaService;

        public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Upload(IFormFile file)
{
    Console.WriteLine("========== UPLOAD START ==========");

    var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);

    Console.WriteLine($"UserId = {userId}");
    Console.WriteLine($"File = {file?.FileName}");
    Console.WriteLine($"File Length = {file?.Length}");
    Console.WriteLine($"ContentType = {file?.ContentType}");

    if (userId == null)
    {
        Console.WriteLine("UPLOAD ERROR: USER NOT LOGIN");
        return Unauthorized();
    }

    if (file == null || file.Length == 0)
    {
        Console.WriteLine("UPLOAD ERROR: FILE EMPTY");
        return BadRequest("Vui lòng chọn file.");
    }

    try
    {
        var url = await _mediaService.UploadAsync(
            file,
            userId.Value);

        Console.WriteLine($"UPLOAD SUCCESS: {url}");
        Console.WriteLine("========== UPLOAD END ==========");

        return Json(new
        {
            success = true,
            url = url
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine("========== UPLOAD ERROR ==========");
        Console.WriteLine(ex.ToString());
        Console.WriteLine("===================================");

        return StatusCode(
            500,
            "Có lỗi xảy ra khi upload ảnh.");
    }
}
    }
}