using BestStoreMVC.Models;
using BestStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestStoreMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Ragistraction()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ragistraction(Users user)
        {
            if (ModelState.IsValid)
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Registration successful!";
                return RedirectToAction("Login");
            }
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Users loginUser)
        {
            
                var user = _context.Users
                    .FirstOrDefault(u => u.Email == loginUser.Email && u.Password == loginUser.Password);

                if (user != null)
                {
                    TempData["Message"] = "Login successful!";
                HttpContext.Session.SetString("UserEmail", user.Email);
                return RedirectToAction("Index", "Products");
                }
                else
                {
                TempData["Message"] = "Invalid email or password";
                return RedirectToAction("Login");
                }
            
             
            return View(loginUser);
        }
         
    }
}
