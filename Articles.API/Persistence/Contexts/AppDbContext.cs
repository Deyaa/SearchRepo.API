using Articles.API.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Articles.API.Persistence.Contexts
{
    /// <summary>
    /// 
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        public DbSet<Category> Categories { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbSet<ArticleCategory> ArticlesCategory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbSet<Article> Articles { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
