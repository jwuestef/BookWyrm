using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookWyrm.Data.ViewModels
{
    public class ApplicationUserIndexViewModel
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public int Balance { get; set; }

        public string Barcode { get; set; }

        public string Role { get; set; }

    }
}
