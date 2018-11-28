using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BookWyrm.Data.ViewModels
{
    public class ApplicationUserCreateViewModel
    {
        // Strings are nullable, so string-based properties are OPTIONAL BY DEFAULT. Add [Required] if you want it mandatory. 
        // Integers aren't nullable, so others are REQUIRED BY DEFAULT. Add ? to be int? to make it optional

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(255, ErrorMessage = "First Name must be less than 255 characters.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(255, ErrorMessage = "Last Name must be less than 255 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "That's not a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Mailing address is required.")]
        public string Address { get; set; }

        [Display(Name = "Date of Birth")]
        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime BirthDate { get; set; }

        public int Balance { get; set; }

        [Required(ErrorMessage = "Barcode is required.")]
        public string Barcode { get; set; }

        [Display(Name = "Hidden Notes")]
        public string HiddenNotes { get; set; }

        [Required(ErrorMessage = "The user must have a role assigned.")]
        public string RoleId { get; set; }

        [Display(Name = "Role")]
        //[Required(ErrorMessage = "The user must have a role assigned.")]
        public List<IdentityRole> Roles { get; set; }




    }
}
