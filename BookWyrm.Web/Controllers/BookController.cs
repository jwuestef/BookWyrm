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
            // Turn the "Book" model we get from the database into the appropriate view model
            IEnumerable<BookViewModel> allBooks = _bookDb.Books.Select(b => new BookViewModel
            {
                BookId = b.BookId,
                Title = b.Title,
                Author = b.Author,
                YearPublished = b.YearPublished,
                Genre = b.Genre,
                Keywords = b.Keywords,
                Description = b.Keywords,
                Barcode = b.Barcode,
                ISBN = b.ISBN,
                MinAgeReq = b.MinAgeReq,
                HiddenNotes = b.HiddenNotes
            });
            return View(allBooks);
        }



        // GET: Book/Details/5
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book foundBook = _bookDb.Books.Find(id);
            if (foundBook == null)
            {
                return HttpNotFound();
            }
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
                HiddenNotes = foundBook.HiddenNotes
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



        //// GET: Book/Edit/5
        //[HttpGet]
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Book book = _bookDb.Books.Find(id);
        //    if (book == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(book);
        //}



        //// POST: Book/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "BookId,Title,Author,YearPublished,Genre,Keywords,Description,ISBN,Barcode,MinAgeReq,HiddenNotes")] Book book)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _bookDb.Entry(book).State = EntityState.Modified;
        //        _bookDb.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(book);
        //}



        //// GET: Book/Delete/5
        //[HttpGet]
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Book book = _bookDb.Books.Find(id);
        //    if (book == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(book);
        //}



        //// POST: Book/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Book book = _bookDb.Books.Find(id);
        //    _bookDb.Books.Remove(book);
        //    _bookDb.SaveChanges();
        //    return RedirectToAction("Index");
        //}



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
