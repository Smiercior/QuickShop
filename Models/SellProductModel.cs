using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace QuickShop.Models
{
    public class SellProductModel
    {
        [Required]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Product name should be longer than 4 characters and shorter than 100 characters")]
        public string Name {get; set;}

        [Required]
        [StringLength(100000, MinimumLength = 4, ErrorMessage = "Product description should be longer than 4 characters and shorter than 100000 characters")]
        public string Description {get; set;}

        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Category should be longer than 1 characters and shorter than 100 characters")]
        public string Category {get; set;}

        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Condition name should be longer than 1 characters and shorter than 100 characters")]
        public string Condition {get; set;}

        [Required]
        // [Range(0.10, 10000)]
        public string Price {get; set;}

        [Required]
        [Range(1, 10000)]
        public int Amount {get; set;}

        public List<IFormFile>? imageFiles {get; set;}

        public List<String> DeliveryTypeCheckboxes {get; set;}

        public Dictionary<String, String> DeliveryTypePrices {get; set;}
    }
}