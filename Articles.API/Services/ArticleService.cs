using Articles.API.Domain.Models;
using Articles.API.Domain.Repositories;
using Articles.API.Domain.Services;
using Articles.API.Domain.Services.Communication;
using Articles.API.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Articles.API.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articlesRepository;

        public ArticleService(IArticleRepository articlesRepository)
        {
            _articlesRepository = articlesRepository;
        }
        public async Task<IEnumerable<ArticleCategory>> ListAsync()
        {
            return await _articlesRepository.ListAsync();
        }

        public async Task<IEnumerable<ArticleCategory>> GetCategoryArticlesAsync(int categoryId)
        {
            return await _articlesRepository.GetCategoryArticlesAsync(categoryId);
        }

        public async Task<ArticleResponse> UpdateFavoriteAsync(int id, ArticleCategory articleCategory)
        {
            var existingArticle = await _articlesRepository.FindByIdAsync(id);

            if (existingArticle == null)
                return new ArticleResponse("Article not found.");

            if(existingArticle.IsFavorite == true)
                existingArticle.IsFavorite = false;
            else
                existingArticle.IsFavorite = true;

            try
            {
                _articlesRepository.Update(existingArticle);
                await _articlesRepository.CompleteAsync();
                return new ArticleResponse(articleCategory);
            }
            catch (Exception ex)
            {
                return new ArticleResponse($"An error occurred when updating the article: {ex.Message}");
            }
        }

        public async Task<IEnumerable<ArticleCategory>> ListFavoritesAsync()
        {
            return await _articlesRepository.ListFavoriteAsync();
        }
    }
}
