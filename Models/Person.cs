using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace QuickShop.Models
{
    public class Person : IdentityUser
    {
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Firstname should be longer than 1 characters and shorter than 100 characters")]
        public string Firstname {get; set;}

        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Firstname should be longer than 1 characters and shorter than 100 characters")]
        public string Surname {get; set;}

        [StringLength(100, MinimumLength = 1, ErrorMessage = "City should be longer than 1 characters and shorter than 100 characters")]
        public string City {get; set;}

        [StringLength(100, MinimumLength = 1, ErrorMessage = "Street should be longer than 1 characters and shorter than 100 characters")]
        public string Street {get; set;}

        [StringLength(100, MinimumLength = 1, ErrorMessage = "Apartment number should be longer than 1 characters and shorter than 100 characters")]
        public string ApartmentNumber {get; set;}

        public virtual ICollection<Product> Products {get; set;}

        public virtual ICollection<ProductTransaction> ProductTransactions {get; set;}

        public virtual ICollection<ProductRating> ProductRatings {get; set;}

        public virtual ICollection<Chat> Chats {get; set;}

        public virtual ICollection<ChatEntry> ChatEntries {get; set;}
    }
}