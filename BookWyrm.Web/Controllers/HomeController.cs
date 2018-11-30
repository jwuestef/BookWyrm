using System;
using System.Collections.Generic;
using System.Globalization;
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
                return Json(new { message = "Invalid user barcode, please try again" });
            }

            // We received a barcode, search the database for this user
            using (IdentityDb _identityDb = new IdentityDb())
            {
                ApplicationUser foundUser = _identityDb.Users.Where(u => u.Barcode == userBarcode).FirstOrDefault();
                if (foundUser == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { message = "Invalid user barcode, please try again" });
                }

                // Get the list of this user's currently checked out books
                using (BookDb _bookDb = new BookDb())
                {
                    var currentlyCheckedOutBooks = _bookDb.Borrowings
                        .Where(bw => bw.UserId == foundUser.Id && bw.CheckInDateTime == null)
                        .Join(_bookDb.Books,
                            bw => bw.BookId,
                            bk => bk.BookId,
                            (borrowing, book) =>
                            new
                            {
                                Title = book.Title,
                                Author = book.Author,
                                DueDate = borrowing.DueDate.ToString(),
                                IsLate = borrowing.DueDate < DateTime.Now
                            }
                        ).ToList();


                    return Json(new
                    {
                        fullName = foundUser.FirstName + " " + foundUser.LastName,
                        userId = foundUser.Id,
                        currentlyCheckedOutBooks = currentlyCheckedOutBooks
                    });
                }
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KioskBookBarcode(string userId, string bookBarcode)
        {
            if (String.IsNullOrWhiteSpace(userId) || String.IsNullOrWhiteSpace(bookBarcode))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = "Invalid user or book barcode, please try again" });
            }

            // We received a barcode, search the database for this book
            using (BookDb _bookDb = new BookDb())
            {
                Book foundBook = _bookDb.Books.Where(b => b.Barcode == bookBarcode).FirstOrDefault();
                if (foundBook == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { message = "Invalid book barcode, please try again" });
                }

                // We also have been given a userId, search the database for this user
                using (IdentityDb _identityDb = new IdentityDb())
                {
                    ApplicationUser foundUser = _identityDb.Users.Find(userId);
                    if (foundUser == null)
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Json(new { message = "Invalid user barcode, please try again" });
                    }

                    // Check to make sure this user doesn't already have this book checked out - no duplicate checkouts allowed
                    var isAlreadyCurrentlyCheckedOutBySameUser = _bookDb.Borrowings.Any(bw => 
                        bw.BookId == foundBook.BookId
                        && bw.UserId == foundUser.Id
                        && bw.CheckInDateTime == null
                    );
                    if (isAlreadyCurrentlyCheckedOutBySameUser)
                    {
                        Response.StatusCode = (int)HttpStatusCode.Conflict;
                        return Json(new { message = "You already have this book checked out" });
                    }

                    // Check to make sure this book isn't checked out by someone else - no duplicate checkouts allowed
                    var isAlreadyCurrentlyCheckedOutBySomeoneElse = _bookDb.Borrowings.Any(bw =>
                        bw.BookId == foundBook.BookId
                        && bw.CheckInDateTime == null
                    );
                    if (isAlreadyCurrentlyCheckedOutBySomeoneElse)
                    {
                        Response.StatusCode = (int)HttpStatusCode.Conflict;
                        return Json(new { message = "Someone else has this book checked out - please contact a librarian" });
                    }

                    // Make sure the user is old enough to meet the book's minimum age requirement
                    var today = DateTime.Today;
                    var age = today.Year - foundUser.BirthDate.Year;
                    if (foundUser.BirthDate > today.AddYears(-age))
                        age--;

                    if (age < foundBook.MinAgeReq)
                    {
                        Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        return Json(new { message = "You do not meet the minimum age requirement to check out this book" });
                    }

                    // Create an entry in the borrowing table
                    Borrowing newBorrowing = new Borrowing()
                    {
                        BorrowingId = Guid.NewGuid(),
                        UserId = userId,
                        BookId = foundBook.BookId,
                        CheckOutDateTime = DateTime.Now,
                        DueDate = DateTime.Now.AddDays(14),
                        CheckInDateTime = null
                    };
                    _bookDb.Borrowings.Add(newBorrowing);
                    _bookDb.SaveChanges();

                    // Check out for this book complete, return the book's information
                    return Json(new
                    {
                        dueDate = newBorrowing.DueDate.ToString("MMM dd yyyy hh:mmtt"),
                        bookDetails = new
                        {
                            Title = foundBook.Title,
                            Author = foundBook.Author
                        }
                    });

                }
            }
        }







    }
}