using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace QuickShop.Models
{
    public class Condition
    {
        [Key]
        public int Id {get; set;}

        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Condition name should be longer than 1 characters and shorter than 100 characters")]
        public string Name {get; set;}

        public virtual ICollection<Product> Products {get; set;}
    }
}