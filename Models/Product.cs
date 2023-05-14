#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace QuickShop.Models
{
    public class Product
    {
        [Key]
        public int Id {get; set;}

        [Required]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Product name should be longer than 4 characters and shorter than 100 characters")]
        public string Name {get; set;}

        [Required]
        [Range(0, 10000)]
        public int Amount {get; set;}

        [Required]
        [StringLength(100000, MinimumLength = 4, ErrorMessage = "Product description should be longer than 4 characters and shorter than 100000 characters")]
        public string Description {get; set;}

        public string ParametersLink {get; set;}

        [ForeignKey("Person")]
        public string PersonId {get; set;}
        public virtual Person Person {get; set;}

        [ForeignKey("Category")]
        public int? CategoryId {get; set;}
        public virtual Category Category {get; set;}

        [ForeignKey("Condition")]
        public int? ConditionId {get; set;}
        public virtual Condition Condition {get; set;}

        public virtual ICollection<ProductImage> ProductImages {get; set;}

        public virtual ICollection<ProductPrice> ProductPrices {get; set;}

        public virtual ICollection<ProductRating> ProductRatings {get; set;}

        public virtual ICollection<ProductTransaction> ProductTransactions {get; set;}
    }
}