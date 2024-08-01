using Microsoft.AspNetCore.Mvc;
using UPC_DropDown.Models;

namespace UPC_DropDown.Controllers
{
    public class ChartsController : Controller
    {
        public IActionResult ChartsList()
        {
            var charts = new List<Chart>
        {
            new Chart { UserName = "Sajjad", UserPassword = "SJD123", UserEmail = "SJD@example.com", UserExperience="20", UserSalary=500000},
            new Chart { UserName = "Yonus", UserPassword = "YNS456", UserEmail = "yns@example.com",UserExperience="13",UserSalary= 200000},
            new Chart { UserName = "Akram", UserPassword = "ARM789", UserEmail = "arm@example.com",UserExperience="10",UserSalary=80000 },
            new Chart { UserName = "Sadiq", UserPassword = "sk123", UserEmail = "sk@example.com",UserExperience="3",UserSalary = 5000 },
            new Chart { UserName = "Hammad Jasir", UserPassword = "hj123", UserEmail = "hj@example.com",UserExperience="6",UserSalary= 25000},
            new Chart { UserName = "Mr.KHAN", UserPassword = "MK123", UserEmail = "MK@example.com",UserExperience="8",UserSalary=30000 }

        };

            return View(charts);
        }
    }

}
