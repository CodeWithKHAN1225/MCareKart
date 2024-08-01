using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CartController : Controller
{
    private readonly ApplicationDbContext _db;

    public CartController(ApplicationDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        var cartItems = _db.CartItem.Include(c => c.Product).ToList();
        var totalAmount = cartItems.Sum(item => item.FinalPrice);
        ViewData["TotalAmount"] = totalAmount;

        return View(cartItems);
    }

    [HttpPost]
    public IActionResult AddToCart(int productId)
    {
        int userId = 2; // Replace with the actual logged-in user ID
        var cartItem = _db.CartItem.FirstOrDefault(c => c.ProductID == productId && c.UserID == userId);

        if (cartItem == null)
        {
            cartItem = new CartItem
            {
                ProductID = productId,
                UserID = userId,
                Quantity = 1
            };
            _db.CartItem.Add(cartItem);
        }
        else
        {
            cartItem.Quantity++;
        }

        _db.SaveChanges();
        return Json(new { success = true });
    }

    [HttpPost]
    public IActionResult DeleteCart(int cartId)
    {
        var cartItem = _db.CartItem.FirstOrDefault(c => c.CartId == cartId);
        if (cartItem != null)
        {
            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
                _db.SaveChanges();
                return Json(new { success = true, quantity = cartItem.Quantity });
            }
            else
            {
                _db.CartItem.Remove(cartItem);
                _db.SaveChanges();
                return Json(new { success = true, quantity = 0 });
            }
        }
        return Json(new { success = false, message = "Item not found" });
    }
}
