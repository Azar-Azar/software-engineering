using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using software_engineering.Data;
using software_engineering.Models;
using System;

namespace software_engineering.Controllers
{
    public class AddUsers : Controller
    {
        private readonly AppDBContext appDBcontext;

        public AddUsers(AppDBContext context)
        {
            appDBcontext = context;
        }
       
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
            appDBcontext.Add(User);
            await appDBcontext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
