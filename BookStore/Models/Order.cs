using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Order
    {
        public Order()
        {
            OrderDetails = new List<OrderDetails>();
        }

        public int Id { get; set; }
        [Display(Name="Order No")]
        public string OrderNo { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Phone No")]
        [Phone]
        public string Phone { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Display(Name ="Delivery Address")]
        public string Address { get; set; }
        [Display(Name="Order Date")]
        public DateTime OrderDate { get; set; }

        public virtual List<OrderDetails> OrderDetails { get; set; }


    }
}
