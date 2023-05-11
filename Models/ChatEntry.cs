using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickShop.Models
{
    public class ChatEntry
    {
        [Key]
        public int Id {get; set;}

        [Required]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "Text should be longer than 1 characters and shorter than 5000 characters")]
        public string text {get; set;}

        [ForeignKey("Chat")]
        public int ChatId {get; set;}
        public virtual Chat Chat {get; set;}

        [ForeignKey("Person")]
        public string PersonId {get; set;}
        public virtual Person Person {get; set;}
    }
}