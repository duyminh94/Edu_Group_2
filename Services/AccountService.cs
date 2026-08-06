using BlogPlatform.Data;
using BlogPlatform.Models;
using BlogPlatform.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Services
{
    public class AccountService : IAccountService
    {
        private readonly BlogDbContext context;
        private readonly IPasswordService passwordService;

        public AccountService(BlogDbContext context, IPasswordService passwordService)
        {
            this.context = context;
            this.passwordService = passwordService;
        }

        public async Task<string> RegisterAsync(RegisterViewModel model)
        {
            var userNameLower = model.UserName.Trim().ToLower();

            if (await context.Users.AnyAsync(u => u.UserName.ToLower() == userNameLower))
            {
                return "Username already exists.";
            }

            var email = model.Email.Trim().ToLower();
            if (await context.Users.AnyAsync(u => u.Email.ToLower() == email))
            {
                return "Email already exists.";
            }

            var readerRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Reader");
            if (readerRole == null)
            {
                return "Reader role not found.";
            }

            var user = new User
            {
                UserName = userNameLower,
                Email = email,
                DisplayName = model.DisplayName.Trim(),
                PasswordHash = passwordService.Hash(model.Password),
                RoleId = readerRole.Id,
                IsLocked = false,
                CreatedAt = DateTime.Now
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
            return "User registered successfully.";
        }
        public async Task<User?> ValidateLoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }
            var userNameLower = username.Trim().ToLower();
            var user = await context.Users.
                Include(u => u.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == userNameLower);
            if (user == null || user.IsLocked)
            {
                return null;
            }
            if (!passwordService.Verify(password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var userNameLower = username.Trim().ToLower();
            return await context.Users.
               Include(u => u.Role)
               .AsNoTracking()
               .FirstOrDefaultAsync(u => u.UserName == userNameLower);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await context.Users.
                Include(u => u.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        // Cập nhật hồ sơ cá nhân
        public async Task<string> UpdateProfileAsync(int userId, string displayName, string? bio, string? avatarUrl)
        {
            var user = await context.Users.FindAsync(userId);
            if (user == null)
            {
                return "User not found.";
            }
            user.DisplayName = displayName.Trim();
            user.Bio = bio?.Trim();
            user.AvatarUrl = avatarUrl?.Trim();
            await context.SaveChangesAsync();
            return "Profile updated successfully.";
        }

        // Danh sách người dùng cho Admin (Issue #13)

        public async Task<List<UserListItemViewModel>> GetAllAsync()
        {
            return await context.Users
                .Include(u => u.Role)
                .Select(u => new UserListItemViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    DisplayName = u.DisplayName,
                    Email = u.Email,
                    RoleName = u.Role.Name,
                    IsLocked = u.IsLocked,
                    CreatedAt = u.CreatedAt,
                    PostCount = u.Posts.Count
                })
                .OrderByDescending(u => u.Id)
                .ToListAsync();
        }

        public async Task<string> ChangeRoleAsync(int userId, int newRoleId, int currentAdminId)
        {
            var user = await context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
            if(user == null)
            {
                return "User not found.";
            }

            var newRole = await context.Roles.FindAsync(newRoleId);
            if (newRole == null) 
            {
                return "Role not found.";
            }

            if(user.Role.Name == "Admin" && newRole.Name != "Admin")
            {   
                var adminCount = await context.Users.CountAsync(u => u.Role.Name == "Admin");
                if (adminCount <= 1) {
                    return "Cannot change role. There must be at least one Admin.";
                }
                return "You cannot change your own role.";
            }
            user.RoleId = newRoleId;
            await context.SaveChangesAsync();
            return "Successfully changed role.";
        }

        public async Task<string> ToggleLockAsync(int userId, int currentAdminId)
        {
            if (userId == currentAdminId)
            {
                return "You cannot lock/unlock your own account.";
            }
            var user = await context.Users.FindAsync(userId);
            if (user == null)
            {
                return "User not found.";
            }
            
            user.IsLocked = !user.IsLocked;
            await context.SaveChangesAsync();
            return user.IsLocked ? "User account locked." : "User account unlocked.";
        }
    }
}
