using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickShop.Models
{
    public class Category
    {
        [Key]
        public int Id {get; set;}

        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Category should be longer than 1 characters and shorter than 100 characters")]
        public string Name {get; set;}

        public virtual ICollection<Product> Products {get; set;}
    }
}