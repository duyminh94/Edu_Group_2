using BlogPlatform.Data;
using BlogPlatform.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký dịch vụ MVC (Controllers và Views)
builder.Services.AddControllersWithViews();

// 2. Đăng ký Session — dự án tự quản đăng nhập, không dùng ASP.NET Core Identity
//    IdleTimeout 30 phút đúng theo quy tắc 3.9 trong business-rules.md
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;      // chặn JavaScript đọc cookie session (chống XSS đánh cắp phiên)
    options.Cookie.IsEssential = true;
});

// 3. Đọc chuỗi kết nối Database từ appsettings.json
//    Giá trị thật nằm trong appsettings.Development.json — xem README Bước 2
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 4. Đăng ký DbContext kết nối SQL Server
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlServer(connectionString));

// 5. Đăng ký Service tầng nghiệp vụ trong DI Container
//    Scoped  = mỗi HTTP request một instance. Dùng cho service có nhận BlogDbContext,
//              vì DbContext cũng là Scoped — service không được sống lâu hơn nó.
//    Singleton = một instance duy nhất cho cả ứng dụng. Chỉ dùng được cho service
//              không đụng database và không giữ trạng thái riêng theo request.
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IInteractionService, InteractionService>();
builder.Services.AddScoped<IBlogSettingService, BlogSettingService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddSingleton<IPasswordService, PasswordService>();
builder.Services.AddSingleton<IHtmlSanitizerService, HtmlSanitizerService>();

builder.Services.AddScoped<ITaxonomyService, TaxonomyService>();

var app = builder.Build();

// 6. Cấu hình HTTP Request Pipeline (Middleware) — thứ tự khai báo chính là thứ tự chạy
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/User/Blog/Error");
}

// TODO Khu B: bỏ comment dòng dưới SAU KHI viết xong action BlogController.Error.
//       Dòng này bắt lỗi 403 (sai quyền) và 404 (sai đường dẫn) rồi render ra trang
//       có layout, thay vì trang trắng của Kestrel. Bật sớm khi chưa có action Error
//       sẽ làm lỗi 404 thành lỗi chồng lỗi.
// app.UseStatusCodePagesWithReExecute("/User/Blog/Error", "?code={0}");

// Phục vụ file tĩnh trong wwwroot.
// CẦN NẮM: giữ UseStaticFiles chứ không bỏ đi chỉ dùng MapStaticAssets, vì ảnh người
// dùng upload vào wwwroot/uploads được sinh ra LÚC CHẠY — MapStaticAssets chỉ biết
// những file đã có sẵn lúc build.
app.UseStaticFiles();

app.UseRouting();

// Bật Middleware Session — bắt buộc đặt sau UseRouting và trước khi map route
app.UseSession();

// Ghi chú: CoreDay05 có app.UseAuthorization() ở đây. Dự án này KHÔNG cần, vì phân quyền
// làm bằng filter tự viết SessionAuthorizeAttribute chứ không dùng ASP.NET Core Identity.
// Thêm vào cũng không sai, chỉ là middleware chạy không.

app.MapStaticAssets();

// 7. Route cho URL thân thiện dạng slug (theo thiết kế trong blog-platform-erd.md)
//    CẦN NẮM: route khớp theo ĐÚNG THỨ TỰ khai báo. Bốn route này phải đứng trước
//    route "default" ở mục 9, vì "default" bắt gần như mọi đường dẫn 2 đoạn.
app.MapControllerRoute(
    name: "post-detail",
    pattern: "post/{slug}",
    defaults: new { area = "User", controller = "Blog", action = "Detail" });

app.MapControllerRoute(
    name: "author-profile",
    pattern: "author/{username}",
    defaults: new { area = "User", controller = "Blog", action = "Author" });

app.MapControllerRoute(
    name: "category",
    pattern: "category/{slug}",
    defaults: new { area = "User", controller = "Blog", action = "Category" });

app.MapControllerRoute(
    name: "tag",
    pattern: "tag/{slug}",
    defaults: new { area = "User", controller = "Blog", action = "Tag" });

// 8. Route cho các Area (Admin, Author, User)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller}/{action=Index}/{id?}");

// 9. Route mặc định — trang chủ "/" là danh sách bài viết trong Area User
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Blog}/{action=Index}/{id?}",
    defaults: new { area = "User" });

// Tự động seed dữ liệu mẫu khi khởi chạy nếu DB chưa có dữ liệu
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
    await DbSeeder.SeedAsync(dbContext);
}

app.Run();

