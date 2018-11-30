using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookWyrm.Data.DataContexts;
using BookWyrm.Data.Models;
using BookWyrm.Data.ViewModels;

namespace BookWyrm.Web.Controllers
{
    public class BookController : Controller
    {
        private BookDb _bookDb = new BookDb();



        // GET: Book
        [HttpGet]
        public ActionResult Index()
        {
            IEnumerable<BookViewModel> allBooks = _bookDb.Books.Select(book => new BookViewModel
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Author = book.Author,
                    YearPublished = book.YearPublished,
                    Genre = book.Genre,
                    Keywords = book.Keywords,
                    Description = book.Keywords,
                    Barcode = book.Barcode,
                    ISBN = book.ISBN,
                    MinAgeReq = book.MinAgeReq,
                    HiddenNotes = book.HiddenNotes,
                    Availability = !_bookDb.Borrowings.Any(bw => bw.BookId == book.BookId && bw.CheckInDateTime == null)
                });
            return View(allBooks);
        }



        // GET: Book/Details/5
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Book foundBook = _bookDb.Books.Find(id);
            if (foundBook == null)
                return HttpNotFound();

            BookViewModel bookViewModel = new BookViewModel()
            {
                BookId = foundBook.BookId,
                Title = foundBook.Title,
                Author = foundBook.Author,
                YearPublished = foundBook.YearPublished,
                Genre = foundBook.Genre,
                Keywords = foundBook.Keywords,
                Description = foundBook.Description,
                Barcode = foundBook.Barcode,
                ISBN = foundBook.ISBN,
                MinAgeReq = foundBook.MinAgeReq,
                HiddenNotes = foundBook.HiddenNotes,
                Availability = !_bookDb.Borrowings.Any(bw => bw.BookId == foundBook.BookId && bw.CheckInDateTime == null)
            };
            return View(bookViewModel);
        }



        // GET: Book/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }



        // POST: Book/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BookViewModel bookViewModel)
        {
            if (ModelState.IsValid)
            {
                Book newBook = new Book()
                {
                    Title = bookViewModel.Title,
                    Author = bookViewModel.Author,
                    YearPublished = bookViewModel.YearPublished,
                    Genre = bookViewModel.Genre,
                    Keywords = bookViewModel.Keywords,
                    Description = bookViewModel.Description,
                    Barcode = bookViewModel.Barcode,
                    ISBN = bookViewModel.ISBN,
                    MinAgeReq = bookViewModel.MinAgeReq,
                    HiddenNotes = bookViewModel.HiddenNotes
                };
                _bookDb.Books.Add(newBook);
                _bookDb.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bookViewModel);
        }



        // GET: Book/Edit/5
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Book foundBook = _bookDb.Books.Find(id);
            if (foundBook == null)
                return HttpNotFound();

            BookViewModel bookViewModel = new BookViewModel()
            {
                BookId = foundBook.BookId,
                Title = foundBook.Title,
                Author = foundBook.Author,
                YearPublished = foundBook.YearPublished,
                Genre = foundBook.Genre,
                Keywords = foundBook.Keywords,
                Description = foundBook.Description,
                Barcode = foundBook.Barcode,
                ISBN = foundBook.ISBN,
                MinAgeReq = foundBook.MinAgeReq,
                HiddenNotes = foundBook.HiddenNotes,
                Availability = !_bookDb.Borrowings.Any(bw => bw.BookId == foundBook.BookId && bw.CheckInDateTime == null)
            };

            return View(bookViewModel);
        }



        // POST: Book/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BookViewModel bookViewModel)
        {
            if (ModelState.IsValid)
            {
                Book foundBookToUpdate = _bookDb.Books.Find(bookViewModel.BookId);

                if (foundBookToUpdate == null)
                    return HttpNotFound();

                foundBookToUpdate.Title = bookViewModel.Title;
                foundBookToUpdate.Author = bookViewModel.Author;
                foundBookToUpdate.YearPublished = bookViewModel.YearPublished;
                foundBookToUpdate.Genre = bookViewModel.Genre;
                foundBookToUpdate.Keywords = bookViewModel.Keywords;
                foundBookToUpdate.Description = bookViewModel.Description;
                foundBookToUpdate.Barcode = bookViewModel.Barcode;
                foundBookToUpdate.ISBN = bookViewModel.ISBN;
                foundBookToUpdate.MinAgeReq = bookViewModel.MinAgeReq;
                foundBookToUpdate.HiddenNotes = bookViewModel.HiddenNotes;

                _bookDb.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bookViewModel);
        }



        // GET: Book/Delete/5
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Book foundBook = _bookDb.Books.Find(id);
            if (foundBook == null)
                return HttpNotFound();

            BookViewModel bookViewModel = new BookViewModel()
            {
                BookId = foundBook.BookId,
                Title = foundBook.Title,
                Author = foundBook.Author,
                YearPublished = foundBook.YearPublished,
                Genre = foundBook.Genre,
                Keywords = foundBook.Keywords,
                Description = foundBook.Description,
                Barcode = foundBook.Barcode,
                ISBN = foundBook.ISBN,
                MinAgeReq = foundBook.MinAgeReq,
                HiddenNotes = foundBook.HiddenNotes,
                Availability = !_bookDb.Borrowings.Any(bw => bw.BookId == foundBook.BookId && bw.CheckInDateTime == null)
            };

            return View(bookViewModel);
        }



        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book foundBook = _bookDb.Books.Find(id);
            _bookDb.Books.Remove(foundBook);
            _bookDb.SaveChanges();
            // TODO: delete all entries in the borrowing table
            return RedirectToAction("Index");
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _bookDb.Dispose();
            }
            base.Dispose(disposing);
        }



    }
}
