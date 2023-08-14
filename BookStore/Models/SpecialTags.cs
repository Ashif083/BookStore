using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class SpecialTags
    {
        public int Id { get; set; }
        [Required]
        [Display(Name ="Tag-Name")]
        public string TagName { get; set; }
    }
}
