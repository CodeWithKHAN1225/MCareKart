using Microsoft.AspNetCore.Mvc;
using System.Linq;
using UPC_DropDown.Models;

namespace UPC_DropDown.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AccountController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        public IActionResult SubmitLogin(LoginModel loginmodel)
        {
            User userObj = _dbContext.Users.FirstOrDefault(p => p.UserName == loginmodel.UserName && p.UserPassword == loginmodel.UserPassword);
            if (userObj == null)
            {
                ModelState.AddModelError("", "Entered username or password is incorrect.");
                return View("Login", loginmodel);
            }
            else
            {
                return RedirectToAction("Dashboard", "Home");
            }
        }

        public IActionResult RegisterForm()
        {
            var model = new RegisterModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult SaveUser(RegisterModel registerModel)
        {
            User newUser = new User
            {
                UserName = registerModel.UserName,
                UserPassword = registerModel.UserPassword,
                UserEmail = registerModel.UserEmail
            };

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }
    }
}

