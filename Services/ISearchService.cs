using BlogPlatform.ViewModel;

namespace BlogPlatform.Services
{
    public interface ISearchService
    {
        Task<SearchViewModel> SearchAsync(SearchViewModel model);
    }
}
