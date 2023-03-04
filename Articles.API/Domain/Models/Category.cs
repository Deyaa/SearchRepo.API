using System.ComponentModel.DataAnnotations;

namespace Articles.API.Domain.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
