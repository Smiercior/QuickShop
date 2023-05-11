using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickShop.Models
{
    public class Chat
    {
        [Key]
        public int Id {get; set;}

        [ForeignKey("ProductTransaction")]
        public int ProductTransactionId {get; set;}
        public virtual ProductTransaction ProductTransaction {get; set;}

        public virtual ICollection<ChatEntry> ChatEntries {get; set;}
    }
}
