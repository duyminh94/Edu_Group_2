using BlogPlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Data
{
    // DbContext chính của ứng dụng — quản lý toàn bộ 12 bảng nghiệp vụ
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {
        }

        // ===== Khai báo 12 bảng =====
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<User> Users => Set<User>();
        public DbSet<BlogSetting> BlogSettings => Set<BlogSetting>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<PostTag> PostTags => Set<PostTag>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<PostLike> PostLikes => Set<PostLike>();
        public DbSet<Bookmark> Bookmarks => Set<Bookmark>();
        public DbSet<PostView> PostViews => Set<PostView>();
        public DbSet<MediaFile> MediaFiles => Set<MediaFile>();

        // Cấu hình quy tắc xoá cho từng quan hệ
        // SQL Server không cho phép nhiều đường cascade cùng trỏ về 1 bảng,
        // nên các quan hệ xuất phát từ User đều phải đặt Restrict để chặn bớt đường thứ hai
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // ===== Seed dữ liệu Role =====
builder.Entity<Role>().HasData(
    new Role
    {
        Id = 1,
        Name = "Admin",
        Description = "Quản trị viên hệ thống"
    },
    new Role
    {
        Id = 2,
        Name = "Author",
        Description = "Tác giả bài viết"
    },
    new Role
    {
        Id = 3,
        Name = "Reader",
        Description = "Người đọc"
    }
);
            // ===== Role → User =====
            // Không cho xoá vai trò khi vẫn còn người dùng đang giữ vai trò đó
            builder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== User → BlogSetting (quan hệ 1-1) =====
            // Xoá người dùng thì xoá luôn cấu hình giao diện của họ
            builder.Entity<BlogSetting>()
                .HasOne(b => b.User)
                .WithOne(u => u.BlogSetting)
                .HasForeignKey<BlogSetting>(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== Category → Post =====
            // Xoá chuyên mục thì bài viết vẫn còn, chỉ mất phân loại
            builder.Entity<Post>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // ===== User → Post =====
            // Không cho xoá người dùng khi họ vẫn còn bài viết
            builder.Entity<Post>()
                .HasOne(p => p.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== Comment =====
            // Xoá bài viết thì xoá luôn bình luận của bài đó
            builder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Xoá người dùng thì KHÔNG xoá bình luận — chặn đường cascade thứ hai tới Comments
            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Bình luận trả lời bình luận — self-reference bắt buộc dùng Restrict
            builder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== PostTag =====
            builder.Entity<PostTag>()
                .HasOne(pt => pt.Post)
                .WithMany(p => p.PostTags)
                .HasForeignKey(pt => pt.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PostTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.PostTags)
                .HasForeignKey(pt => pt.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== PostLike =====
            builder.Entity<PostLike>()
                .HasOne(l => l.Post)
                .WithMany(p => p.PostLikes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PostLike>()
                .HasOne(l => l.User)
                .WithMany(u => u.PostLikes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== Bookmark =====
            builder.Entity<Bookmark>()
                .HasOne(b => b.Post)
                .WithMany(p => p.Bookmarks)
                .HasForeignKey(b => b.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Bookmark>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookmarks)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== PostView =====
            builder.Entity<PostView>()
                .HasOne(v => v.Post)
                .WithMany(p => p.PostViews)
                .HasForeignKey(v => v.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PostView>()
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== MediaFile =====
            builder.Entity<MediaFile>()
                .HasOne(m => m.Post)
                .WithMany(p => p.MediaFiles)
                .HasForeignKey(m => m.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<MediaFile>()
                .HasOne(m => m.UploadedBy)
                .WithMany(u => u.MediaFiles)
                .HasForeignKey(m => m.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
