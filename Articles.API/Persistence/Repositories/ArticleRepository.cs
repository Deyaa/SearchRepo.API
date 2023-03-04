using Articles.API.Domain.Enums;
using Articles.API.Domain.Models;
using Articles.API.Domain.Repositories;
using Articles.API.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Articles.API.Persistence.Repositories
{
    public class ArticleRepository : BaseRepository, IArticleRepository
    {
        public ArticleRepository(AppDbContext context) : base(context)
        {

        }
        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Article> FindByIdAsync(int id)
        {
            return await _context.Articles.FindAsync(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ArticleCategory>> ListAsync()
        {
            return await _context.ArticlesCategory
                .FromSqlRaw($"EXEC {eStoredProcedures.splGetArticles.ToString()}")
                .ToListAsync();
        }
        public async Task<IEnumerable<ArticleCategory>> ListFavoriteAsync()
        {
            return await _context.ArticlesCategory
                .FromSqlRaw($"EXEC {eStoredProcedures.splGetFavoriteArticles.ToString()}")
                .ToListAsync();
        }
        public async Task<IEnumerable<ArticleCategory>> GetCategoryArticlesAsync(int categoryId)
        {
            IDictionary<string, object> paramsDic = new Dictionary<string, object>();
            paramsDic.Add("id", categoryId);
            var parameters = GenerateStoredProcedureParams(paramsDic);
            string commandText = GenerateCommandText(eStoredProcedures.splGetCategoryArticles.ToString(), parameters);

            return await _context.ArticlesCategory
                .FromSqlRaw(commandText, parameters)
                .ToListAsync();
        }

        public async void UpdateArticleFavoriteAsync(int articleId, bool isFavorite)
        {
            IDictionary<string, object> paramsDic = new Dictionary<string, object>();
            paramsDic.Add("id", articleId);
            paramsDic.Add("isFavorite", isFavorite);
            var parameters = GenerateStoredProcedureParams(paramsDic);
            string commandText = GenerateCommandText(eStoredProcedures.splUpdateArticleFavorite.ToString(), parameters);

            //return await _context.Articles.e
        }

        public void Update(Article article)
        {
            _context.Articles.Update(article);
        }
    }
}
