
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SeamlessLaunchpad.Models;
using SeamlessLaunchpad.ViewModel.SeamlessLaunchpad.ViewModels;

namespace SeamlessLaunchpad.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly SLPADDBContext _context;


        public AccountController(SLPADDBContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            _context = context;
            this.signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true,
                    Association = model.Association

                };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    
                    //AspNetUsers found = (AspNetUsers)_context.AspNetUsers.Where(u => u.Email == model.Email);
                    //found.Association = model.Association;
                    //_context.Entry(found).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    //_context.Update(found);
                    //_context.SaveChanges();
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Launchpad");
                }
                //foreach (var error in result.Errors)
                //{
                //    ModelState.AddModelError("", error.Description);
                //}
            }
            return View();
        }
    }
}
