using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickShop.Models
{
    public class DeliveryTypePrice
    {
        [Key]
        public int Id {get; set;}

        [Required]
        [Range(0, 10000)]
        public float Price {get; set;}

        [ForeignKey("DeliveryType")]
        public int DeliveryTypeId {get; set;}
        public virtual DeliveryType DeliveryType {get; set;}

        public virtual ICollection<ProductTransaction> ProductTransactions {get; set;}
    }
}