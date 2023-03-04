using Articles.API.Domain.Models;
using Articles.API.Domain.Services.Communication;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Articles.API.Domain.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IArticleService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ArticleCategory>> ListAsync();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<IEnumerable<ArticleCategory>> GetCategoryArticlesAsync(int categoryId);
        Task<ArticleResponse> UpdateFavoriteAsync(int id, ArticleCategory articleCategory);
        Task<IEnumerable<ArticleCategory>> ListFavoritesAsync();
    }
}
