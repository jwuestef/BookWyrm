using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookWyrm.Data.Models;

namespace BookWyrm.Data.ViewModels
{
    public class SearchViewModel
    {
        public string SearchTerm { get; set; }

        public IEnumerable<Book> SearchResults { get; set; }


    }
}
