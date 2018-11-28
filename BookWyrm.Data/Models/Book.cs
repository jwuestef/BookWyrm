using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookWyrm.Data.Models
{
    public class Book
    {
        public int BookId { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public int YearPublished { get; set; }

        public string Genre { get; set; }

        public string Keywords { get; set; }

        public string Description { get; set; }

        public string ISBN { get; set; }

        public string Barcode { get; set; }

        public int MinAgeReq { get; set; }

        public string HiddenNotes { get; set; }

    }
}
