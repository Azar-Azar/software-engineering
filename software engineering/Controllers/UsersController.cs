using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using software_engineering.Data;
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
        public async Task<IActionResult> Index()
        {
            //get the list from the DB asynchronously 
            var user = await appDBcontext.User.ToListAsync();
            
            return View(user);
        }

        //adding a user
        //the show form
        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }
        //add a user to the database
        [HttpPost]
        public async Task<IActionResult> AddUser(Users user)
        {
            appDBcontext.Add(user);
            await appDBcontext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Delete a user
        public async Task<IActionResult> Delete(int? id)
        {
            //check if the user id is not null 
            if (id == null)
            {
                return NotFound();
            }

            //find a user that has the ID "id"
            var user = await appDBcontext.User.FirstOrDefaultAsync(userIterator => userIterator.ID == id);

            //check if a user has beeen found
            if (user == null)
            {
                return NotFound();
            }

            return View(user); // show a conformation page
        }

        [HttpPost, ActionName("Delete")]
        
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
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = " In the meantime, this user has been deleted.";
                return RedirectToAction(nameof(Index));

            }

            var user = await appDBcontext.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();

            }

            return View(user); // returns the edit form
        }


        // POST: Books/Edit/XX
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, Users user)
        {
            if (id != user.ID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    //make the changes in the DBContext 
                    appDBcontext.Update(user);

                    //apply the changes made in the DbContext to the database
                    await appDBcontext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    // what does happen if the book doesn’t exist??
                    throw;
                }
            }

            return View(user);
        }




    }
}
