using BlogPlatform.Data;
using BlogPlatform.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký MVC
builder.Services.AddControllersWithViews();

// Cấu hình Session — dự án tự quản đăng nhập, không dùng ASP.NET Core Identity
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;      // chặn JavaScript đọc cookie session (chống XSS đánh cắp phiên)
    options.Cookie.IsEssential = true;
});

// Đăng ký DbContext trỏ tới SQL Server
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// TODO: đăng ký các service nghiệp vụ (Scoped)
// builder.Services.AddScoped<IAccountService, AccountService>();
// builder.Services.AddScoped<IPostService, PostService>();
// builder.Services.AddScoped<ICommentService, CommentService>();
// builder.Services.AddScoped<ISearchService, SearchService>();
// builder.Services.AddScoped<IInteractionService, InteractionService>();
// builder.Services.AddScoped<IBlogSettingService, BlogSettingService>();
// builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
// builder.Services.AddScoped<IMediaService, MediaService>();
// builder.Services.AddSingleton<IPasswordService, PasswordService>();
// builder.Services.AddSingleton<IHtmlSanitizerService, HtmlSanitizerService>();

var app = builder.Build();

// Cấu hình pipeline xử lý request
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/User/Blog/Error");
}

app.UseStaticFiles();

app.UseRouting();

// Bật Session — phải đặt sau UseRouting và trước MapControllerRoute
app.UseSession();

// Route cho các Area (Admin, Author, User)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller}/{action=Index}/{id?}");

// Route mặc định — trang chủ là danh sách bài viết trong Area User
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Blog}/{action=Index}/{id?}",
    defaults: new { area = "User" });

app.Run();
