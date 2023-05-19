using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickShop.Models
{
    public class DeliveryType
    {
        [Key]
        public int Id {get; set;}

        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Delivery type name should be longer than 1 characters and shorter than 100 characters")]
        public string Name {get; set;}

        [Required]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Delivery type details should be longer than 1 characters and shorter than 1000 characters")]
        public string Details {get; set;}

        public virtual ICollection<DeliveryTypePrice> DeliveryTypePrices {get; set;}
    }
}