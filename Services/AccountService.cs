using BlogPlatform.Data;
using BlogPlatform.Models;
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
    }
}
