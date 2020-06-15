using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LoginAndRegistration.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace LoginAndRegistration.Controllers
{
    public class HomeController : Controller
    {
        private Context dbContext;
        public HomeController(Context context)
        {
            dbContext = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RegisterUser(User user)
        {
            if (ModelState.IsValid)
            {
                if (dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                dbContext.Users.Add(user);
                dbContext.SaveChanges();
                return RedirectToAction("Login");
            }
            return View("Index");
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult ProcessLogin(LoginUser user)
        {
            if(ModelState.IsValid)
            {
                User dbUser = dbContext.Users.FirstOrDefault(u => u.Email == user.Email);
                if(dbUser != null)
                {
                    Console.WriteLine("\n\n Passed Email Verification!\n\n");
                    PasswordHasher<LoginUser> Hasher = new PasswordHasher<LoginUser>();
                    if((Hasher.VerifyHashedPassword(user, dbUser.Password, user.Password)) != 0)
                    {
                        Console.WriteLine("\n\n Passed Password Verification!\n\n");
                        HttpContext.Session.SetInt32("UserId", dbUser.UserId);
                        return RedirectToAction("Success");
                    }
                }
            }
            ModelState.AddModelError("Email", "Invalid Email/Password");
            return View("Login");
        }

        public IActionResult Success()
        {
            if(HttpContext.Session.GetInt32("UserId") != null)
            {
                return View();
            }
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
