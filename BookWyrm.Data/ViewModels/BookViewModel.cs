using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookWyrm.Data.ViewModels
{
    public class BookViewModel
    {

        // Strings are nullable, so string-based properties are OPTIONAL BY DEFAULT. Add [Required] if you want it mandatory. 
        // Integers aren't nullable, so others are REQUIRED BY DEFAULT. Add ? to be int? to make it optional


        public int BookId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required.")]
        public string Author { get; set; }

        [Display(Name = "Year Published")]
        [Required(ErrorMessage = "Year published is required.")]
        public int YearPublished { get; set; }

        [Required(ErrorMessage = "Genre is required.")]
        public string Genre { get; set; }

        [Required(ErrorMessage = "Keywords are required.")]
        public string Keywords { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Barcode is required.")]
        public string Barcode { get; set; }

        public string ISBN { get; set; }

        [Display(Name = "Min. Patron Age")]
        public int MinAgeReq { get; set; }

        [Display(Name = "Hidden Notes")]
        public string HiddenNotes { get; set; }

        public bool? Availability { get; set; }
    }
}
