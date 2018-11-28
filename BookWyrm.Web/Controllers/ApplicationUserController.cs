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
            else
            {
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



        //// POST: ApplicationUsers/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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
                    UserManager.AddToRole(user.Id, applicationUserCreateViewModel.RoleId);
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



        //// GET: ApplicationUsers/Edit/5
        // //[HttpGet]   - STILL DO THIS ELSEWHERE
        //public ActionResult Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ApplicationUser applicationUser = _identityDb.Users.Find(id);
        //    if (applicationUser == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(applicationUser);
        //}



        //// POST: ApplicationUsers/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Barcode,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _identityDb.Entry(applicationUser).State = EntityState.Modified;
        //        _identityDb.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(applicationUser);
        //}



        // GET: ApplicationUsers/Delete/5
        [HttpGet]
        public ActionResult Delete(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ApplicationUser foundUser = _identityDb.Users.Find(id);

            if (foundUser == null)
                return HttpNotFound();
            else
            {
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
        }



        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser applicationUser = _identityDb.Users.Find(id);
            _identityDb.Users.Remove(applicationUser);
            _identityDb.SaveChanges();
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