using BlogPlatform.Models;
using BlogPlatform.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(BlogDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            // Seed Roles
            if (!await context.Roles.AnyAsync())
            {
                await context.Roles.AddRangeAsync(
                    new Role { Id = 1, Name = "Admin", Description = "Quản trị toàn hệ thống" },
                    new Role { Id = 2, Name = "Author", Description = "Viết bài, duyệt bình luận trên bài của mình" },
                    new Role { Id = 3, Name = "Reader", Description = "Đọc bài, bình luận, thích và lưu bài" }
                );
                await context.SaveChangesAsync();
            }

            // Seed Users (Mật khẩu mặc định: Admin@123 -> BCrypt hash: $2a$11$fbtobL02kmcj1lbjczAyP.kSBK0gqaxfj0Cc39m9Amx8NEdOccQ8e)
            if (!await context.Users.AnyAsync())
            {
                var defaultPasswordHash = "$2a$11$fbtobL02kmcj1lbjczAyP.kSBK0gqaxfj0Cc39m9Amx8NEdOccQ8e";
                var users = new List<User>
                {
                    new User { Id = 1, UserName = "admin", Email = "admin@blog.local", PasswordHash = defaultPasswordHash, DisplayName = "Quản trị viên", Bio = "Tài khoản quản trị hệ thống", RoleId = 1 },
                    new User { Id = 2, UserName = "minh", Email = "minh@blog.local", PasswordHash = defaultPasswordHash, DisplayName = "Duy Minh", Bio = "Fresher developer, thích viết về lập trình", RoleId = 2 },
                    new User { Id = 3, UserName = "lan", Email = "lan@blog.local", PasswordHash = defaultPasswordHash, DisplayName = "Ngọc Lan", Bio = "Yêu thích thiết kế giao diện", RoleId = 2 },
                    new User { Id = 4, UserName = "hoa", Email = "hoa@blog.local", PasswordHash = defaultPasswordHash, DisplayName = "Thanh Hòa", Bio = "Độc giả thường xuyên", RoleId = 3 }
                };
                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();

                // BlogSettings cho tác giả
                await context.BlogSettings.AddRangeAsync(
                    new BlogSetting { UserId = 2, ThemeName = "light", PrimaryColor = "#4f46e5", FontFamily = "Plus Jakarta Sans", Tagline = "Ghi chép trên đường học lập trình" },
                    new BlogSetting { UserId = 3, ThemeName = "minimal", PrimaryColor = "#06b6d4", FontFamily = "Outfit", Tagline = "Góc nhỏ về thiết kế" }
                );
                await context.SaveChangesAsync();
            }

            // Seed Categories
            if (!await context.Categories.AnyAsync())
            {
                await context.Categories.AddRangeAsync(
                    new Category { Id = 1, Name = "Lập trình", Slug = "lap-trinh", Description = "Ngôn ngữ và kỹ thuật lập trình" },
                    new Category { Id = 2, Name = "Công nghệ", Slug = "cong-nghe", Description = "Tin tức và xu hướng công nghệ" },
                    new Category { Id = 3, Name = "Thiết kế", Slug = "thiet-ke", Description = "Giao diện, trải nghiệm người dùng" },
                    new Category { Id = 4, Name = "Học tập", Slug = "hoc-tap", Description = "Kinh nghiệm và phương pháp học" },
                    new Category { Id = 5, Name = "Đời sống", Slug = "doi-song", Description = "Chuyện đời thường, kỹ năng sống" }
                );
                await context.SaveChangesAsync();
            }

            // Seed Tags
            if (!await context.Tags.AnyAsync())
            {
                await context.Tags.AddRangeAsync(
                    new Tag { Id = 1, Name = "ASP.NET Core", Slug = "aspnet-core" },
                    new Tag { Id = 2, Name = "C#", Slug = "csharp" },
                    new Tag { Id = 3, Name = "SQL Server", Slug = "sql-server" },
                    new Tag { Id = 4, Name = "Entity Framework", Slug = "entity-framework" },
                    new Tag { Id = 5, Name = "JavaScript", Slug = "javascript" },
                    new Tag { Id = 6, Name = "CSS", Slug = "css" },
                    new Tag { Id = 7, Name = "Bảo mật", Slug = "bao-mat" },
                    new Tag { Id = 8, Name = "Kinh nghiệm", Slug = "kinh-nghiem" }
                );
                await context.SaveChangesAsync();
            }

            // Seed Posts
            if (!await context.Posts.AnyAsync())
            {
                var posts = new List<Post>
                {
                    new Post
                    {
                        Id = 1,
                        Title = "Bắt đầu với ASP.NET Core MVC",
                        Slug = "bat-dau-voi-aspnet-core-mvc",
                        Summary = "Hướng dẫn dựng project MVC đầu tiên từ con số không",
                        Content = "<p>ASP.NET Core MVC chia ứng dụng thành ba phần: Model, View và Controller. Bài viết này hướng dẫn chi tiết cách dựng dự án từ đầu, cấu hình Routing và làm việc với Razor View.</p>",
                        FeaturedImageUrl = "https://images.unsplash.com/photo-1555066931-4365d14bab8c?auto=format&fit=crop&w=800&q=80",
                        CategoryId = 1,
                        AuthorId = 2,
                        Status = PostStatus.Published,
                        PublishedAt = DateTime.UtcNow.AddDays(-10),
                        ViewCount = 120,
                        LikeCount = 8,
                        CommentCount = 2
                    },
                    new Post
                    {
                        Id = 2,
                        Title = "Hiểu về Entity Framework Core Code First",
                        Slug = "hieu-ve-entity-framework-core-code-first",
                        Summary = "Code First, migration và cách EF Core sinh bảng từ class C#",
                        Content = "<p>EF Core cho phép viết class C# rồi tự sinh ra bảng trong database. Quá trình tạo Migration giúp theo vết sự thay đổi cấu trúc dữ liệu theo thời gian.</p>",
                        FeaturedImageUrl = "https://images.unsplash.com/photo-1542831371-29b0f74f9713?auto=format&fit=crop&w=800&q=80",
                        CategoryId = 1,
                        AuthorId = 2,
                        Status = PostStatus.Published,
                        PublishedAt = DateTime.UtcNow.AddDays(-5),
                        ViewCount = 85,
                        LikeCount = 5,
                        CommentCount = 1
                    },
                    new Post
                    {
                        Id = 3,
                        Title = "Chọn màu và phông chữ chuẩn cho Website",
                        Slug = "chon-mau-va-phong-chu-chuan-cho-website",
                        Summary = "Vài nguyên tắc chọn bảng màu và font chữ để trang web hiện đại và dễ nhìn",
                        Content = "<p>Độ tương phản giữa chữ và nền nên đạt tối thiểu 4.5:1. Sử dụng hệ màu Tailored HSL và font chữ không chân như Plus Jakarta Sans mang lại cảm giác vô cùng cao cấp.</p>",
                        FeaturedImageUrl = "https://images.unsplash.com/photo-1507238691740-187a5b1d37b8?auto=format&fit=crop&w=800&q=80",
                        CategoryId = 3,
                        AuthorId = 3,
                        Status = PostStatus.Published,
                        PublishedAt = DateTime.UtcNow.AddDays(-2),
                        ViewCount = 64,
                        LikeCount = 12,
                        CommentCount = 0
                    }
                };

                await context.Posts.AddRangeAsync(posts);
                await context.SaveChangesAsync();

                // Seed PostTags
                await context.PostTags.AddRangeAsync(
                    new PostTag { PostId = 1, TagId = 1 },
                    new PostTag { PostId = 1, TagId = 2 },
                    new PostTag { PostId = 2, TagId = 1 },
                    new PostTag { PostId = 2, TagId = 4 },
                    new PostTag { PostId = 3, TagId = 6 }
                );

                // Seed Comments
                await context.Comments.AddRangeAsync(
                    new Comment { PostId = 1, UserId = 4, ParentCommentId = null, Content = "Bài viết rất dễ hiểu, cảm ơn tác giả!", Status = CommentStatus.Approved, CreatedAt = DateTime.UtcNow.AddDays(-9) },
                    new Comment { PostId = 1, UserId = 2, ParentCommentId = 1, Content = "Cảm ơn bạn đã đọc bài nhé!", Status = CommentStatus.Approved, CreatedAt = DateTime.UtcNow.AddDays(-8) },
                    new Comment { PostId = 2, UserId = 4, ParentCommentId = null, Content = "Phần Migration giải thích rất hay.", Status = CommentStatus.Approved, CreatedAt = DateTime.UtcNow.AddDays(-4) }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
