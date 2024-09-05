using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SearchRepo.API.Domain.Models;
using SearchRepo.API.Domain.Services;
using SearchRepo.API.Extensions;
using SearchRepo.API.Filters;
using SearchRepo.API.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SearchRepo.API.Controllers
{
    [ApiController]
    public class SearchRepoController : Controller
    {
        private readonly ILogger<SearchRepoController> _logger;
        private readonly IMapper _mapper;
        private readonly ISearchService _searchService;
        private IConfiguration _configuration;


        public SearchRepoController(ILogger<SearchRepoController> logger, IMapper mapper, IConfiguration configuration
            , ISearchService searchService)
        {
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
            _searchService = searchService;
        }
        /// <summary>
        /// Search string by using github api
        /// </summary>
        /// <param name="q">search string</param>
        /// <returns></returns>
        [Route("search/{q}")]
        [HttpGet]
        [ServiceFilter(typeof(AuthorizeFilter))]
        [ProducesResponseType(typeof(GalleryItemsResource), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGalleryItemsBySearchAsync(string q)
        {
            var galleryItems = await _searchService.GetGithubReposAsync(q);
            if (galleryItems == null)
                return NoContent();

            var resources = _mapper.Map<IEnumerable<GalleryItem>, IEnumerable<GalleryItemsResource>>(galleryItems);
            return Ok(resources);
        }
        /// <summary>
        /// Bookmark item retrieved from gallery
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        [Route("bookmark-repo")]
        [HttpPost]
        [ServiceFilter(typeof(AuthorizeFilter))]
        [ProducesResponseType(typeof(BookmarkRepoResource), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> BookmarkRepositoryAsync([FromBody] BookmarkRepoResource resource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            var repo = _mapper.Map<BookmarkRepoResource, BookmarkRepo>(resource);
            await _searchService.UpdateBookmarkRepositoryAsync(repo);
            return Ok(resource);
        }
        /// <summary>
        /// All bookmarked items list
        /// </summary>
        /// <returns></returns>
        [Route("bookmarked-repos")]
        [HttpGet]
        [ServiceFilter(typeof(AuthorizeFilter))]
        [ProducesResponseType(typeof(BookmarkRepoResource), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IEnumerable<BookmarkRepoResource>> GetBookmarkedRepositoriesAsync()
        {
            var bookmarkedRepos = await _searchService.GetBookmarkedRepositoriesAsync();
            var resources = _mapper.Map<IEnumerable<BookmarkRepo>, IEnumerable<BookmarkRepoResource>>(bookmarkedRepos);
            return resources;
        }
    }
}
