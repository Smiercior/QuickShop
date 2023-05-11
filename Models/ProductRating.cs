using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickShop.Models
{
    public class ProductRating
    {
        [Key]
        public int Id {get; set;}

        [Required]
        [Range(1,5)]
        public int Stars {get; set;}

        [Required]
        [StringLength(5000, MinimumLength = 4, ErrorMessage = "Comment should be longer than 4 characters and shorter than 5000 characters")]
        public string Comment {get; set;}

        [ForeignKey("Product")]
        public int ProductId {get; set;}
        public virtual Product Product {get; set;}

        [ForeignKey("Person")]
        public string PersonId {get; set;}
        public virtual Person Person {get; set;}
    }
}