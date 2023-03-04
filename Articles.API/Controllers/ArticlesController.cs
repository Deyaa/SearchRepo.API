using Articles.API.Domain.Models;
using Articles.API.Domain.Services;
using Articles.API.Resources;
using Articles.API.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Articles.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class ArticlesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IArticleService _articleService;
        private readonly ILogger<ArticlesController> _logger;
        private readonly IMapper _mapper;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryService"></param>
        /// <param name="articleService"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public ArticlesController(ICategoryService categoryService, IArticleService articleService, 
            ILogger<ArticlesController> logger, IMapper mapper)
        {
            _categoryService = categoryService;
            _articleService = articleService;
            _logger = logger;
            _mapper = mapper;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("categories")]
        [HttpGet]
        [ProducesResponseType(typeof(CategoryResource), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IEnumerable<CategoryResource>> GetCategoriesAsync()
        {
            var categories = await _categoryService.ListAsync();
            var resources = _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryResource>>(categories);
            return resources;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("articles")]
        [HttpGet]
        [ProducesResponseType(typeof(ArticleResource), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IEnumerable<ArticleResource>> GetArticlesAsync()
        {
            var articles = await _articleService.ListAsync();
            var resources = _mapper.Map<IEnumerable<ArticleCategory>, IEnumerable<ArticleResource>>(articles);
            return resources;
        }
        [Route("favorite-articles")]
        [HttpGet]
        [ProducesResponseType(typeof(ArticleResource), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IEnumerable<ArticleResource>> GetFavoriteArticlesAsync()
        {
            var favoriteArticles = await _articleService.ListFavoritesAsync();
            var resources = _mapper.Map<IEnumerable<ArticleCategory>, IEnumerable<ArticleResource>>(favoriteArticles);
            return resources;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("articles/{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(ArticleResource), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IEnumerable<ArticleResource>> GetCategoryArticlesAsync(int id)
        {
            var articles = await _articleService.GetCategoryArticlesAsync(id);
            var resources = _mapper.Map<IEnumerable<ArticleCategory>, IEnumerable<ArticleResource>>(articles);
            return resources;
        }
        [Route("articles/{id}")]
        [HttpPut]
        [ProducesResponseType(typeof(ArticleResource), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateArticleDetailsAsync(int id, [FromBody] ArticleResource resource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            var article = _mapper.Map<ArticleResource, ArticleCategory>(resource);
            var result = await _articleService.UpdateFavoriteAsync(id, article);

            if (!result.Success)
                return BadRequest(result.Message);

            var articleResource = _mapper.Map<ArticleCategory, ArticleResource>(result.ArticleCategory);
            return Ok(articleResource);
        }
    }
}
