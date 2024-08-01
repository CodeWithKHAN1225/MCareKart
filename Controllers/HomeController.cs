using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UPC_DropDown.Models;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;

   public HomeController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Dashboard()
    {
        var viewModel = new DashboardViewModel
        {
            Products = await _db.Products.ToListAsync(),
            Users = await _db.Users.ToListAsync(),
            Categories = await _db.Categories.ToListAsync()
        };

        return View(viewModel);
    }
}