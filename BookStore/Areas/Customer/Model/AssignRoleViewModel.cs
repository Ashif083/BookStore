using System.ComponentModel.DataAnnotations;

namespace BookStore.Areas.Customer.Model
{
    public class AssignRoleViewModel
    {
        [Required]
        [Display(Name ="User")]
        public string UserId { get; set; }
        
        [Required]
        [Display(Name = "Role")]
        public string RoleId { get; set; }
    }
}
