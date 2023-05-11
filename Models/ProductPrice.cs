using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickShop.Models
{
    public class ProductPrice
    {
        [Key]
        public int Id {get; set;}

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Datetime {get; set;}

        [Required]
        [Range(0.10, 10000)]
        public float Price {get; set;}

        [ForeignKey("Product")]
        public int ProductId {get; set;}
        public virtual Product Product {get; set;}

        public virtual ICollection<ProductTransaction> ProductTransactions {get; set;}
    }
}