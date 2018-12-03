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
                IEnumerable<BookViewModel> searchResult = _bookDb.Books.Where(b =>
                    b.Title.Contains(query) ||
                    b.Author.Contains(query) ||
                    b.Genre.Contains(query) ||
                    b.Keywords.Contains(query) ||
                    b.Description.Contains(query) ||
                    b.Barcode.Contains(query) ||
                    b.ISBN.Contains(query)
                ).Select(book => new BookViewModel
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Author = book.Author,
                    YearPublished = book.YearPublished,
                    Genre = book.Genre,
                    Keywords = book.Keywords,
                    Description = book.Description,
                    Barcode = book.Barcode,
                    ISBN = book.ISBN,
                    MinAgeReq = book.MinAgeReq,
                    HiddenNotes = book.HiddenNotes,
                    Availability = !_bookDb.Borrowings.Any(bw => bw.BookId == book.BookId && bw.CheckInDateTime == null)
                }).ToList();

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
                    bool isAlreadyCurrentlyCheckedOutBySameUser = _bookDb.Borrowings.Any(bw => 
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
                    bool isAlreadyCurrentlyCheckedOutBySomeoneElse = _bookDb.Borrowings.Any(bw =>
                        bw.BookId == foundBook.BookId
                        && bw.CheckInDateTime == null
                    );
                    if (isAlreadyCurrentlyCheckedOutBySomeoneElse)
                    {
                        Response.StatusCode = (int)HttpStatusCode.Conflict;
                        return Json(new { message = "Someone else has this book checked out - please contact a librarian" });
                    }

                    // Check to make sure this user hasn't hit the checkout limit
                    var numCheckouts = _bookDb.Borrowings.Count(bw =>
                        bw.UserId == foundUser.Id && bw.CheckInDateTime == null
                    );
                    if (numCheckouts >= 3)
                    {
                        Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        return Json(new { message = "You have too many books checked out right now" });
                    }

                    // Make sure the user is old enough to meet the book's minimum age requirement
                    DateTime today = DateTime.Today;
                    int age = today.Year - foundUser.BirthDate.Year;
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
                        dueDate = newBorrowing.DueDate.ToString("MMM dd yyyy h:mmtt"),
                        bookDetails = new
                        {
                            Title = foundBook.Title,
                            Author = foundBook.Author
                        }
                    });

                }
            }
        }



        [HttpGet]
        public ActionResult BookCheckIn()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BookCheckIn(string bookBarcode)
        {
            if (String.IsNullOrWhiteSpace(bookBarcode))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = "Invalid book barcode, please try again" });
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

                // Check to make sure this book has been checked out by someone
                Borrowing foundBorrowing = _bookDb.Borrowings.Where(bw =>
                    bw.BookId == foundBook.BookId
                    && bw.CheckInDateTime == null
                ).FirstOrDefault();
                if (foundBorrowing == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.Conflict;
                    return Json(new { message = "This book isn't checked out" });
                }

                // Get the borrower's information
                using (IdentityDb _identityDb = new IdentityDb())
                {
                    ApplicationUser borrower = _identityDb.Users.Where(u => u.Id == foundBorrowing.UserId).FirstOrDefault();
                    if (borrower == null)
                    {
                        Response.StatusCode = (int)HttpStatusCode.Conflict;
                        return Json(new { message = "Could not locate user the book was checked out to" });
                    }

                    // Update the entry in the borrowing table to include today as the check-in date
                    foundBorrowing.CheckInDateTime = DateTime.Now;
                    _bookDb.SaveChanges();

                    int daysLate = 0;
                    if (foundBorrowing.DueDate < foundBorrowing.CheckInDateTime)
                    {
                        double daysLateWithDecimals = (DateTime.Now - foundBorrowing.DueDate).TotalDays;
                        daysLate = (int)Math.Floor(daysLateWithDecimals);
                    }

                    // TODO: Add late fee, if applicable - only for today since today's script already ran on startup
                    // TODO: add in call to chron job on index action of this home controller to make sure balances are up to date

                    // Check in for this book complete, return the book's information
                    return Json(new
                    {
                        Title = foundBook.Title,
                        Author = foundBook.Author,
                        DueDate = foundBorrowing.DueDate.ToString("MMM dd yyyy h:mmtt"),
                        DaysLate = daysLate,
                        Borrower = borrower.FirstName + " " + borrower.LastName
                    });
                }
            }
        }







    }
}