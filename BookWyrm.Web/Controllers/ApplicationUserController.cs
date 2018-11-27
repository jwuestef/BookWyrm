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
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_identityDb));
            var UserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_identityDb));

            if (!roleManager.RoleExists("Kiosk"))
            {
                var role = new IdentityRole();
                role.Name = "Kiosk";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Patron"))
            {
                var role = new IdentityRole();
                role.Name = "Patron";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Librarian"))
            {
                var role = new IdentityRole();
                role.Name = "Librarian";
                roleManager.Create(role);
            }



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
                                      Role = string.Join(",", p.RoleNames)
                                  });


            return View(allUsersIncludingRoles);

        }



        //// GET: ApplicationUsers/Details/5
        // //[HttpGet]   - STILL DO THIS ELSEWHERE
        //public ActionResult Details(string id)
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



        // GET: ApplicationUsers/Create
        [HttpGet]
        public ActionResult Create()
        {

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_identityDb));
            var roles = roleManager.Roles.ToList();

            var viewModel = new ApplicationUserCreateViewModel
            {
                Roles = roles,
                Balance = 0
            };

            return View(viewModel);
        }



        //// POST: ApplicationUsers/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create(ApplicationUserCreateViewModel applicationUserCreateViewModel)
        //{


        //    if (ModelState.IsValid)
        //    {
        //        //var SelectedCampus = db.Campuses.Single(camp => camp.CampusId == createUserViewModel.CampusId);
        //        //var SelectedDepartment = db.Departments.Single(dept => dept.DepartmentId == createUserViewModel.DepartmentId);
        //        //var SelectedCohort = db.Cohorts.SingleOrDefault(cohort => cohort.CohortId == createUserViewModel.CohortId);

        //        var user = new ApplicationUser()
        //        {
        //            FirstName = applicationUserCreateViewModel.FirstName,
        //            LastName = applicationUserCreateViewModel.LastName,
        //            Email = applicationUserCreateViewModel.Email,
        //            Barcode = applicationUserCreateViewModel.Barcode
        //        };
        //        var UserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_identityDb));

        //        var result = await UserManager.CreateAsync(user, "Testing123!");
        //        if (result.Succeeded)
        //        {
        //            UserManager.AddToRole(user.Id, applicationUserCreateViewModel.RoleId);
        //            return RedirectToAction("Index", "ApplicationUsers");
        //        }
        //        else
        //        {
        //            foreach (var error in result.Errors)
        //            {
        //                ModelState.AddModelError("", error);
        //            }
        //        }
        //    }



        //    // If we got this far, something failed, redisplay form
        //    ModelState.AddModelError("", "ERROR - unable to save user to DB for unknown reason. Redisplay the form with the same values...");

        //    var viewModel = new ApplicationUserCreateViewModel
        //    {
        //        FirstName = applicationUserCreateViewModel.FirstName,
        //        LastName = applicationUserCreateViewModel.LastName,
        //        Email = applicationUserCreateViewModel.Email,
        //        CampusId = applicationUserCreateViewModel.CampusId,
        //        Campuses = _identityDb.Campuses.ToList(),
        //        DepartmentId = applicationUserCreateViewModel.DepartmentId,
        //        Departments = _identityDb.Departments.ToList(),
        //        CohortId = applicationUserCreateViewModel.CohortId,
        //        Cohorts = _identityDb.Cohorts.ToList(),
        //        Barcode = applicationUserCreateViewModel.Barcode
        //    };

        //    return View(viewModel);





        //}



        //// GET: ApplicationUsers/Edit/5
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



        //// GET: ApplicationUsers/Delete/5
        //public ActionResult Delete(string id)
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



        //// POST: ApplicationUsers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    ApplicationUser applicationUser = _identityDb.Users.Find(id);
        //    _identityDb.Users.Remove(applicationUser);
        //    _identityDb.SaveChanges();
        //    return RedirectToAction("Index");
        //}



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