using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BookWyrm.Data.DataContexts;
using BookWyrm.Data.Models;
using BookWyrm.Data.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BookWyrm.Web.Controllers
{
    public class ApplicationUserController : Controller
    {

        private IdentityDb _identityDb = new IdentityDb();
        public ApplicationUserManager UserManager { get; private set; }



        // GET: ApplicationUsers
        [HttpGet]
        public ActionResult Index()
        {
            // When we visit this User Management page, make sure the appropriate roles exist in the database
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_identityDb));
            ApplicationUserManager UserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_identityDb));

            if (!roleManager.RoleExists("Kiosk"))
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Kiosk";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Patron"))
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Patron";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Librarian"))
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Librarian";
                roleManager.Create(role);
            }



            // We want to display a list of all users on this page, we need to get the appropriate info out of the db
            var allUsersIncludingRoles = (from user in _identityDb.Users
                                  select new
                                  {
                                      Id = user.Id,
                                      FirstName = user.FirstName,
                                      LastName = user.LastName,
                                      Email = user.Email,
                                      PhoneNumber = user.PhoneNumber,
                                      Balance = user.Balance,
                                      Barcode = user.Barcode,
                                      RoleNames = (from userRole in user.Roles
                                                   join role in _identityDb.Roles on userRole.RoleId
                                                   equals role.Id
                                                   select role.Name).ToList()
                                  }).ToList().Select(p => new ApplicationUserIndexViewModel()
                                  {
                                      Id = p.Id,
                                      FirstName = p.FirstName,
                                      LastName = p.LastName,
                                      Email = p.Email,
                                      PhoneNumber = p.PhoneNumber,
                                      Balance = p.Balance,
                                      Barcode = p.Barcode,
                                      Role = string.Join(",", p.RoleNames)
                                  });

            return View(allUsersIncludingRoles);

        }



        // GET: ApplicationUsers/Details/5
        [HttpGet]
        public ActionResult Details(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ApplicationUser foundUser = _identityDb.Users.Find(id);

            if (foundUser == null)
                return HttpNotFound();
             
            // The user was found, but we only have a role ID - we need to get the name of their role
            string roleName = "";
            IdentityUserRole foundRole = foundUser.Roles.FirstOrDefault();
            if (foundRole != null)
                roleName = _identityDb.Roles.Find(foundRole.RoleId).Name;

            ApplicationUserDetailsViewModel applicationUserDetailsViewModel = new ApplicationUserDetailsViewModel()
            {
                Id = id,
                FirstName = foundUser.FirstName,
                LastName = foundUser.LastName,
                Email = foundUser.Email,
                PhoneNumber = foundUser.PhoneNumber,
                Address = foundUser.Address,
                BirthDate = foundUser.BirthDate,
                Balance = foundUser.Balance,
                Barcode = foundUser.Barcode,
                HiddenNotes = foundUser.HiddenNotes,
                RoleName = roleName
            };
            return View(applicationUserDetailsViewModel);
        }



        // GET: ApplicationUsers/Create
        [HttpGet]
        public ActionResult Create()
        {

            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_identityDb));
            List<IdentityRole> roles = roleManager.Roles.ToList();

            ApplicationUserCreateViewModel applicationUserCreateViewModel = new ApplicationUserCreateViewModel
            {
                Roles = roles,
                Balance = 0
            };

            return View(applicationUserCreateViewModel);
        }



        // POST: ApplicationUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ApplicationUserCreateViewModel applicationUserCreateViewModel)
        {

            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser()
                {
                    FirstName = applicationUserCreateViewModel.FirstName,
                    LastName = applicationUserCreateViewModel.LastName,
                    Email = applicationUserCreateViewModel.Email,
                    UserName = applicationUserCreateViewModel.Email,
                    PhoneNumber = applicationUserCreateViewModel.PhoneNumber,
                    Address = applicationUserCreateViewModel.Address,
                    BirthDate = applicationUserCreateViewModel.BirthDate,
                    Balance = applicationUserCreateViewModel.Balance,
                    Barcode = applicationUserCreateViewModel.Barcode,
                    HiddenNotes = applicationUserCreateViewModel.HiddenNotes
                };

                ApplicationUserManager UserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_identityDb));
                IdentityResult result = await UserManager.CreateAsync(user, user.Id);

                if (result.Succeeded)
                {
                    // The .AddToRole method requires the NAME of the role, not the ID
                    // We need to query the DB to get the name
                    string roleName = _identityDb.Roles.Find(applicationUserCreateViewModel.RoleId).Name;
                    UserManager.AddToRole(user.Id, roleName);
                    return RedirectToAction("Index", "ApplicationUser");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }

            // If we got this far, the model state was not valid or the user creation failed (there should be errors in the ModelState)
            // Redisplay form with the same data
            ModelState.AddModelError("", "ERROR - failed to create user.");
            return View(applicationUserCreateViewModel);
        }



        // GET: ApplicationUsers/Edit/5
        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ApplicationUser foundUser = _identityDb.Users.Find(id);

            if (foundUser == null)
                return HttpNotFound();

            // The user was found, but we only have a role ID - we need to get the name of their role
            // Since we are saving the role ID later in the code, we need to account for the case where the role wasn't found - null
            string roleName = "";
            IdentityUserRole foundRole = foundUser.Roles.FirstOrDefault();
            if (foundRole != null)
                roleName = _identityDb.Roles.Find(foundRole.RoleId).Name;
            else
                foundRole = new IdentityUserRole()
                {
                    RoleId = "",
                    UserId = ""
                };

            ApplicationUserEditViewModel applicationUserEditViewModel = new ApplicationUserEditViewModel()
            {
                Id = id,
                FirstName = foundUser.FirstName,
                LastName = foundUser.LastName,
                Email = foundUser.Email,
                PhoneNumber = foundUser.PhoneNumber,
                Address = foundUser.Address,
                BirthDate = foundUser.BirthDate,
                Balance = foundUser.Balance,
                Barcode = foundUser.Barcode,
                HiddenNotes = foundUser.HiddenNotes,
                RoleId = foundRole.RoleId,
                OriginalRoleId = foundRole.RoleId,
                Roles = _identityDb.Roles.ToList()
            };
            return View(applicationUserEditViewModel);
        }



        // POST: ApplicationUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ApplicationUserEditViewModel applicationUserEditViewModel)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser foundUserToUpdate = _identityDb.Users.Find(applicationUserEditViewModel.Id);

                if (foundUserToUpdate == null)
                    return HttpNotFound();

                foundUserToUpdate.FirstName = applicationUserEditViewModel.FirstName;
                foundUserToUpdate.LastName = applicationUserEditViewModel.LastName;
                foundUserToUpdate.Email = applicationUserEditViewModel.Email;
                foundUserToUpdate.UserName = applicationUserEditViewModel.Email;
                foundUserToUpdate.PhoneNumber = applicationUserEditViewModel.PhoneNumber;
                foundUserToUpdate.Address = applicationUserEditViewModel.Address;
                foundUserToUpdate.BirthDate = applicationUserEditViewModel.BirthDate;
                foundUserToUpdate.Balance = applicationUserEditViewModel.Balance;
                foundUserToUpdate.Barcode = applicationUserEditViewModel.Barcode;
                foundUserToUpdate.HiddenNotes = applicationUserEditViewModel.HiddenNotes;

                // Apply the changes (if any) to the database
                ApplicationUserManager UserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_identityDb));
                var result = await UserManager.UpdateAsync(foundUserToUpdate);

                if (result.Succeeded)
                {
                    // If the role changed we need to remove them from their old role and add them to their new role
                    if (applicationUserEditViewModel.OriginalRoleId != applicationUserEditViewModel.RoleId)
                    {
                        // The .RemoveFromRole and .AddToRole methods require the NAME of the role, not the ID...
                        // ...so we need to query the database to get the names

                        // If there is an old role, then remove that role
                        if (applicationUserEditViewModel.OriginalRoleId != null & applicationUserEditViewModel.OriginalRoleId != "")
                        {
                            string oldRoleName = _identityDb.Roles.Find(applicationUserEditViewModel.OriginalRoleId).Name;
                            UserManager.RemoveFromRole(applicationUserEditViewModel.Id, oldRoleName);
                        }
                        // Add them to their new role
                        string newRoleName = _identityDb.Roles.Find(applicationUserEditViewModel.RoleId).Name;
                        UserManager.AddToRole(applicationUserEditViewModel.Id, newRoleName);
                    }

                    return RedirectToAction("Index", "ApplicationUser");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }

            // If we got this far, the model state was not valid or the user creation failed (there should be errors in the ModelState)
            // Redisplay form with the same data
            ModelState.AddModelError("", "ERROR - failed to create user.");
            return View(applicationUserEditViewModel);
        }



        // GET: ApplicationUsers/Delete/5
        [HttpGet]
        public ActionResult Delete(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ApplicationUser foundUser = _identityDb.Users.Find(id);

            if (foundUser == null)
                return HttpNotFound();

            // The user was found, but we only have a role ID - we need to get the name of their role
            string roleName = "";
            IdentityUserRole foundRole = foundUser.Roles.FirstOrDefault();
            if (foundRole != null)
                roleName = _identityDb.Roles.Find(foundRole.RoleId).Name;

            ApplicationUserDeleteViewModel applicationUserDeleteViewModel = new ApplicationUserDeleteViewModel()
            {
                Id = id,
                FirstName = foundUser.FirstName,
                LastName = foundUser.LastName,
                Email = foundUser.Email,
                PhoneNumber = foundUser.PhoneNumber,
                Address = foundUser.Address,
                BirthDate = foundUser.BirthDate,
                Balance = foundUser.Balance,
                Barcode = foundUser.Barcode,
                HiddenNotes = foundUser.HiddenNotes,
                RoleName = roleName
            };
            return View(applicationUserDeleteViewModel);
        }



        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            ApplicationUser applicationUser = _identityDb.Users.Find(id);
            _identityDb.Users.Remove(applicationUser);
            await _identityDb.SaveChangesAsync();
            // TODO: delete all entries in the borrowing table
            return RedirectToAction("Index");
        }



        [HttpGet]
        public ActionResult TransactionTable(string id, string fullName)
        {
            // Find all of the transactions in the table for this user
            using (TransactionDb _transactionDB = new TransactionDb())
            {
                using (BookDb _bookDb = new BookDb())
                {
                    // Can't do a join to get the book's title and author too, since it's in different DB contexts
                    // Instead get the list of all transactions and then loop over it, getting details for each transaction one by one
                    List<Transaction> allTransactionsForThisUser = _transactionDB.Transactions.Where(t => t.PersonId == id).ToList();
                    List<TransactionTableViewModel> allTransactionsForThisUserWithDetails = new List<TransactionTableViewModel>();

                    allTransactionsForThisUser.ForEach(eachTransaction =>
                    {
                        // Get the details for this book
                        var bookDetails = _bookDb.Books.Where(bk => bk.BookId == eachTransaction.BookId).FirstOrDefault();
                        if (bookDetails == null)
                            throw new KeyNotFoundException("BookId not found in database, can not populate table with book title/author.");
                        // Then add this instance to the overall list of view models
                        allTransactionsForThisUserWithDetails.Add(new TransactionTableViewModel()
                        {
                            DateApplied = eachTransaction.DateApplied,
                            Amount = eachTransaction.Amount,
                            BookTitle = bookDetails.Title,
                            BookAuthor = bookDetails.Author,
                            Notes = eachTransaction.Notes
                        });
                    });

                    // Send the user's full name to the view too
                    ViewData["FullName"] = fullName;

                    return View(allTransactionsForThisUserWithDetails);
                }
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTransaction()
        {
            // TODO: implementation
            return RedirectToAction("Index");
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _identityDb.Dispose();
            }
            base.Dispose(disposing);
        }



    }
}