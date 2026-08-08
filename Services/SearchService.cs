using BlogPlatform.Data;
using BlogPlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Services
{
    public class SearchService : ISearchService
    {
        private readonly BlogDbContext context;

        public SearchService(BlogDbContext context)
        {
            this.context = context;
        }
    }
}
