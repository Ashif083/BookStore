using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        [DisplayName("Order")]
        public int OrderId { get; set; }
        [DisplayName("Book")]
        public int BookId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
        [ForeignKey("BookId")]
        public Books Books { get; set; }
    }
}
