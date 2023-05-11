using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickShop.Models
{
    public class ProductImage
    {
        [Key]
        public int Id {get; set;}

        [Required]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Image link should be longer than 4 characters and shorter than 100 characters")]
        public string ImageLink {get; set;}

        [ForeignKey("Product")]
        public int ProductId {get; set;}
        public virtual Product Product {get; set;}
    }
}