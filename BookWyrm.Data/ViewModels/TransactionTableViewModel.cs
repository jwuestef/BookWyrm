using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookWyrm.Data.ViewModels
{
    public class TransactionTableViewModel
    {

        public DateTime DateApplied { get; set; }

        public double Amount { get; set; }

        public string BookTitle { get; set; }

        public string BookAuthor { get; set; }

        public string Notes { get; set; }



    }
}
