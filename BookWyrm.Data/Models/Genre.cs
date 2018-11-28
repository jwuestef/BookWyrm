using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BookWyrm.Data.Models
{
    public static class Genre
    {
        public static IEnumerable<SelectListItem> StateListItems()
        {
            return new List<SelectListItem>
            {
                new SelectListItem() {Text = "Non-Fiction", Value = "NonFiction"},
                new SelectListItem() {Text = "Science Fiction", Value = "ScienceFiction"},
                new SelectListItem() {Text = "Fantasy", Value = "Fantasy"},
                new SelectListItem() {Text = "Adventure", Value = "Adventure"},
                new SelectListItem() {Text = "Action", Value = "Action"},
                new SelectListItem() {Text = "Romance", Value = "Romance"},
                new SelectListItem() {Text = "Drama", Value = "Drama"},
                new SelectListItem() {Text = "Mystery", Value = "Mystery"},
                new SelectListItem() {Text = "Comedy", Value = "Comedy"},
                new SelectListItem() {Text = "Horror", Value = "Horror"},
                new SelectListItem() {Text = "Mythology", Value = "Mythology"}
            };
        }
    }


}
