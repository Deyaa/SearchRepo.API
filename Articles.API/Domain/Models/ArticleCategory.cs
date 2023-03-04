using System.ComponentModel.DataAnnotations;

namespace Articles.API.Domain.Models
{
    public class ArticleCategory
    {
        [Key]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Category { get; set; }
        public bool IsFavorite { get; set; }
        public string CategoryName { get; set; }
    }
}
