using Articles.API.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Articles.API.Domain.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public interface IArticleRepository
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Article> FindByIdAsync(int id);
        void Update(Article article);
        Task CompleteAsync();
        Task<IEnumerable<ArticleCategory>> ListFavoriteAsync();
    }
}
