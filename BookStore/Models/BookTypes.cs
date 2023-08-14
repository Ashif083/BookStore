using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class BookTypes
    {
        public int Id { get; set; }

        [Required]
        [Display(Name ="Genre")]
        public string BookType { get; set; }


    }
}
