using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BookWyrm.Data.ViewModels
{
    public class ApplicationUserDeleteViewModel
    {
        public string Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime BirthDate { get; set; }

        public int Balance { get; set; }

        public string Barcode { get; set; }

        [Display(Name = "Hidden Notes")]
        public string HiddenNotes { get; set; }

        [Display(Name = "Role")]
        public string RoleName { get; set; }

    }
}
