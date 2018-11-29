using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BookWyrm.Data.DataContexts;
using BookWyrm.Data.Models;
using BookWyrm.Data.ViewModels;

namespace BookWyrm.Web.Controllers
{
    public class HomeController : Controller
    {



        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }



        [HttpGet]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }



        [HttpGet]
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



        [HttpGet]
        public ActionResult Kiosk()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KioskUserBarcode(string userBarcode)
        {
            if (String.IsNullOrWhiteSpace(userBarcode))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = "Invalid barcode, please try again" });
            }

            // We received a barcode, search the database for this user
            using (IdentityDb _identityDb = new IdentityDb())
            {
                ApplicationUser foundUser = _identityDb.Users.Where(u => u.Barcode == userBarcode).FirstOrDefault();
                if (foundUser == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { message = "Invalid barcode, please try again" });
                }

                string fullName = foundUser.FirstName + " " + foundUser.LastName;

                // We found the user, reply and tell the user it's okay to start scanning books
                return Json(new {
                    fullName = foundUser.FirstName + " " + foundUser.LastName,
                    userId = foundUser.Id
                });
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KioskBookBarcode(string userId, string bookBarcode)
        {
            if (String.IsNullOrWhiteSpace(userId) || String.IsNullOrWhiteSpace(bookBarcode))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = "Invalid barcode, please try again" });
            }

            // We received a barcode, search the database for this book
            using (BookDb _bookDb = new BookDb())
            {
                Book foundBook = _bookDb.Books.Where(b => b.Barcode == bookBarcode).FirstOrDefault();
                if (foundBook == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { message = "Invalid barcode, please try again" });
                }

                // Now create an entry in the borrowing table
                Borrowing newBorrowing = new Borrowing()
                {
                    UserId = userId,
                    BookId = foundBook.BookId,
                    CheckOutDateTime = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(14),
                    CheckInDateTime = null
                };
                _bookDb.Borrowings.Add(newBorrowing);
                _bookDb.SaveChanges();

                // Check out for this book complete, tell the user to continue
                return Json(newBorrowing);
            }
        }







    }
}