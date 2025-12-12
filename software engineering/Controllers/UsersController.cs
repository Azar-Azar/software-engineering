using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using software_engineering.Data;
using software_engineering.Filters;
using software_engineering.Models;
using System;

namespace software_engineering.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDBContext appDBcontext;

        public UsersController(AppDBContext context)
        {
            appDBcontext = context;
            //create an object of the database
            //a session of the database
        }
        //viewing all the users
       [AdminOnly]
       //User/Index
        public async Task<IActionResult> Index()
        {
            //get the list from the DB asynchronously 
            var user = await appDBcontext.User.ToListAsync();

            return View(user);
        }

        //adding a user

        [HttpGet]
        [AdminOnly]
        public IActionResult AddUser()
        {
            return View();
        }
        //add a user to the database
        [HttpPost]//Users/AddUser
        [AdminOnly]
        public async Task<IActionResult> AddUser(Users user)
        {
            if (ModelState.IsValid)//checks if all data if valid or fill out
            {
                appDBcontext.Add(user);
                await appDBcontext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Please fill out all required fields before submitting the form.";
            return View(user);
        }

        //Delete a user
        [AdminOnly]
        public async Task<IActionResult> Delete(int? id)
        {
            //check if the user id is not null 
            if (id == null)
            {
                TempData["ErrorMessage"] = "This user has been deleted already.";
                return RedirectToAction(nameof(Index));
            }

            //find a user that has the ID "id"
            var user = await appDBcontext.User.FirstOrDefaultAsync(userIterator => userIterator.ID == id);

            //check if a user has beeen found
            if (user == null)
            {
                TempData["ErrorMessage"] = "This user has been deleted already.";
                return RedirectToAction(nameof(Index));
            }

            return View(user); // show a conformation page
        }

        [HttpPost, ActionName("Delete")] //User/Delete
        [AdminOnly] 
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //find a user that has the ID "id"
            var user = await appDBcontext.User.FindAsync(id);

            if (user != null)
            {
                //remove the user from the DBContext
                appDBcontext.User.Remove(user);
                //Apply the changes made in the DbContext to the database
                await appDBcontext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index)); // back to list
        }

        //Edit Users
        [AdminOnly]
        public async Task<IActionResult> EditUser(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "This user has been deleted.";
                return RedirectToAction(nameof(Index));

            }

            var user = await appDBcontext.User.FindAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "This user has been deleted.";
                return RedirectToAction(nameof(Index));

            }

            

            return View(user);
        }



        // POST: Users/EditUser/XX



        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminOnly]

        public async Task<IActionResult> EditUser(int id, Users user)
        {
            if(id != user.ID){ return NotFound(); }
            if (ModelState.IsValid)
            {
                try
                {
                    appDBcontext.Update(user);
                    await appDBcontext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.ID))
                    {
                        TempData["ErrorMessage"] = "The User has been deleted.";
                    return RedirectToAction(nameof(Index));
                    }
                    else { throw; }
                }
            }
            return View(user); 
        }
        
        private bool UserExists(int id)
        {
            return appDBcontext.User.Any(bI => bI.ID == id);
        }



        //Reset Password
        [AdminOnly]
        public async Task<IActionResult> ResetPassword(int? id)
        {
            //check if the user id is not null 
            if (id == null)
            {
                TempData["ErrorMessage"] = "This user has been deleted.";
                return RedirectToAction(nameof(Index));
            }

            //find a user that has the ID "id"
            var user = await appDBcontext.User.FirstOrDefaultAsync(userIterator => userIterator.ID == id);

            //check if a user has beeen found
            if (user == null)
            {
                TempData["ErrorMessage"] = "This user has been deleted.";
                return RedirectToAction(nameof(Index));
            }

            return View(user); // show a conformation page
        }

        [HttpPost, ActionName("ResetPassword")]
        [AdminOnly]

        public async Task<IActionResult> ResetConfirmed(int id)
        {
            //find a user that has the ID "id"
            var user = await appDBcontext.User.FindAsync(id);

            if (user != null)
            {
                //Changes the password to Password123
                user.Password = "Password123";
                //Apply the changes made in the DbContext to the database
                await appDBcontext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index)); // back to list
        }

        //login

        public IActionResult Login()
        {
            return View();
        }

        // POST: 
        [HttpPost]

        public async Task<IActionResult> Login(string email, string password)
        {
            if (email == null)
            {
                TempData["ErrorMessage"] = "Requires an Email ";
                return View();

            }
            if (password == null)
            {
                TempData["ErrorMessage"] = "Requires a Password.";
                return View();

            }


            //find a user that has both the email and password
            var loginUser = await appDBcontext.User.FirstOrDefaultAsync(userIterator => userIterator.Email == email && userIterator.Password == password);
            if (loginUser == null)
            {
                TempData["ErrorMessage"] = "Incorrect Email or Password.";
                return View();

            }
            // Store user info in session
            HttpContext.Session.SetInt32("UserID", loginUser.ID);
            HttpContext.Session.SetString("UserRole", loginUser.Role.ToString());

            switch (loginUser.Role)//redirect based on role to their respective homepages
            {
                case Roles.Admin:
                    return RedirectToAction(nameof(Index));
                case Roles.user:
                    return RedirectToAction("Index", "");
                case Roles.clincian:
                    return RedirectToAction("Index", "");
                default:
                    ModelState.AddModelError("", "Invalid user role");
                    return View();
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // Clear all session data
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> notifications()
        {
            //get the list from the DB asynchronously 
            var Alert1 = await appDBcontext.Alerts.ToListAsync();

            return View(Alert1);
        }
    }
}
