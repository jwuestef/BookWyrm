using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookWyrm.Data.DataContexts;
using BookWyrm.Data.Models;
using BookWyrm.Data.ViewModels;

namespace BookWyrm.Web.Controllers
{
    public class HomeController : Controller
    {



        public ActionResult Index()
        {
            return View();
        }



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }



        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TriggerSearch(string query)
        {
            // Make sure we actually received a string
            if (String.IsNullOrEmpty(query))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            using (BookDb _bookDb = new BookDb())
            {
                // Find matching books in the database
                IEnumerable<Book> searchResult = _bookDb.Books.Where(b =>
                    b.Title.Contains(query) ||
                    b.Author.Contains(query) ||
                    b.Genre.Contains(query) ||
                    b.Keywords.Contains(query) ||
                    b.Description.Contains(query) ||
                    b.Barcode.Contains(query) ||
                    b.ISBN.Contains(query)
                ).ToList();

                SearchViewModel searchResultViewModel = new SearchViewModel() { SearchTerm = query, SearchResults = searchResult };

                return View("SearchResult", searchResultViewModel);
            }
        }



    }
}