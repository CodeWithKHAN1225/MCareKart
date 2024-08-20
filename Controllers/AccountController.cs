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
        public IActionResult SubmitLogin(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", loginModel);
            }

            var userObj = _dbContext.Users
                .FirstOrDefault(p => p.UserName == loginModel.UserName && p.UserPassword == loginModel.UserPassword);

            if (userObj == null)
            {
                ModelState.AddModelError("", "Entered username or password is incorrect.");
                return View("Login", loginModel);
            }

            return RedirectToAction("Dashboard", "Home");
        }

        public IActionResult RegisterForm()
        {
            var model = new RegisterModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult SaveUser(RegisterModel registerModel)
        {
            // Validate the model
            if (!ModelState.IsValid)
            {
                return View("RegisterForm", registerModel);
            }

            // Check if the username or email already exists
            if (_dbContext.Users.Any(u => u.UserName == registerModel.UserName || u.UserEmail == registerModel.UserEmail))
            {
                ModelState.AddModelError("", "Username or Email already exists.");
                return View("RegisterForm", registerModel);
            }

            var newUser = new User
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
