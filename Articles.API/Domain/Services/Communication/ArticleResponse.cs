using Articles.API.Domain.Models;


namespace Articles.API.Domain.Services.Communication
{
    /// <summary>
    /// 
    /// </summary>
    public class ArticleResponse : BaseResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public ArticleCategory ArticleCategory { get; private set; }

        private ArticleResponse(bool success, string message, ArticleCategory articleCategory) : base(success, message)
        {
            ArticleCategory = articleCategory;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="articleCategory">Saved article.</param>
        /// <returns>Response.</returns>
        public ArticleResponse(ArticleCategory articleCategory) : this(true, string.Empty, articleCategory)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public ArticleResponse(string message) : this(false, message, null)
        { }
    }
}
