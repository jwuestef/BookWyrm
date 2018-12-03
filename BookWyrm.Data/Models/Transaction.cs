using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookWyrm.Data.Models
{
    public class Transaction
    {
        // Strings are nullable, so string-based properties are OPTIONAL BY DEFAULT. Add [Required] if you want it mandatory. 
        // Integers aren't nullable, so others are REQUIRED BY DEFAULT. Add ? to be int? to make it optional


        public Guid TransactionId { get; set; }

        public string PersonId { get; set; }

        public DateTime DateApplied { get; set; }

        public double Amount { get; set; }

        public int? BookId { get; set; }

        public string Notes { get; set; }



    }
}
