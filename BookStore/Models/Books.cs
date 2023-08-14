using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BookStore.Models
{
    public class Books
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Book Name")]
        public string BookName { get; set; }
        [Required]
        [Display(Name = "Author Name")]
        public string AuthorName { get; set; }

        public string? Publisher { get; set; }
        [Required]
        [Display(Name = "Genre")]
        public int BookTypeId { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        [Display(Name = "Available Quantity")]
        public int AvailableQuantity { get; set; }

        public string? Image { get; set; }
       
        [Required]
        [Display(Name = "Tag")]
        public int SpecialTagId { get; set; }

        [Display(Name = "Available")]
        public bool IsAvailable { get; set; }


        [ForeignKey("SpecialTagId")]
        public SpecialTags? SpecialTags { get; set; }
        [ForeignKey("BookTypeId")]
        public BookTypes? BookTypes { get; set; }
    }
}
