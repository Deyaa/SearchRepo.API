using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Articles.API.Resources
{
    public class ArticleResource
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Category { get; set; }
        public bool IsFavorite { get; set; }
        public string CategoryName { get; set; }
    }
}
