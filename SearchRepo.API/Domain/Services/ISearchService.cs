using SearchRepo.API.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SearchRepo.API.Domain.Services
{
    public interface ISearchService
    {
        Task<IEnumerable<GalleryItem>> GetGithubReposAsync(string searchInput);
        Task UpdateBookmarkRepositoryAsync(BookmarkRepo bookmarkRepo);
        Task<IEnumerable<BookmarkRepo>> GetBookmarkedRepositoriesAsync();
    }
}
