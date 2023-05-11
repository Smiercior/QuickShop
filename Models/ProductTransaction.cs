using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickShop.Models
{
    public class ProductTransaction
    {
        [Key]
        public int Id {get; set;}

        [Required]
        public bool Paid {get; set;}

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Datetime {get; set;}

        [Required]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Phone number must have 12 characters")]
        public string PhoneNumber {get; set;}

        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "City should be longer than 1 characters and shorter than 100 characters")]
        public string City {get; set;}

        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Street should be longer than 1 characters and shorter than 100 characters")]
        public string Street {get; set;}

        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Apartment number should be longer than 1 characters and shorter than 100 characters")]
        public string ApartmentNumber {get; set;}

        [ForeignKey("DeliveryType")]
        public int? DeliveryTypeId {get; set;}
        public virtual DeliveryType DeliveryType {get; set;}

        [ForeignKey("DeliveryTypePrice")]
        public int? DeliveryTypePriceId {get; set;}
        public virtual DeliveryTypePrice DeliveryTypePrice {get; set;}

        [ForeignKey("Chat")]
        public int ChatId {get; set;}
        public virtual Chat Chat {get; set;}

        [ForeignKey("Product")]
        public int? ProductId {get; set;}
        public virtual Product Product {get; set;}

        [ForeignKey("ProductPrice")]
        public int? ProductPriceId {get; set;}
        public virtual ProductPrice ProductPrice {get; set;}

        [ForeignKey("Person")]
        public string PersonId {get; set;}
        public virtual Person Person {get; set;}
    }
}