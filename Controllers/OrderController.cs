using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UPC_DropDown.Models;

public class OrderController : Controller
{
    private readonly ApplicationDbContext _db;

    public OrderController(ApplicationDbContext db)
    {
        _db = db;
    }

    public IActionResult OrderList()
    {
        var orders = _db.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ToList();
        return View(orders);
    }

    [HttpPost]
    public IActionResult CreateOrder([FromBody] List<int> selectedItems)
    {
        try
        {
            if (selectedItems == null || !selectedItems.Any())
            {
                return Json(new { success = false, message = "No items selected for the order." });
            }

            // Assuming the user is logged in and you have the UserID
            int userId = 2; // Replace with the actual logged-in user ID

            var cartItems = _db.CartItem.Include(c => c.Product)
                                        .Where(c => selectedItems.Contains(c.CartId) && c.UserID == userId)
                                        .ToList();
            if (!cartItems.Any())
            {
                return Json(new { success = false, message = "No items in cart to order" });
            }

            // Generate the next order number from the sequence
            int orderNumber = _db.GetNextOrderNumber();

            var order = new Order
            {
                OrderNumber = orderNumber, // Assign the generated order number
                UserID = userId,
                OrderDate = DateTime.Now,
                TotalPrice = cartItems.Sum(ci => ci.Product.ProductPrice * ci.Quantity),
                OrderItems = cartItems.Select(ci => new OrderItem
                {
                    ProductID = ci.ProductID,
                    Quantity = ci.Quantity,
                    Price = ci.Product.ProductPrice
                }).ToList()
            };

            _db.Orders.Add(order);
            _db.CartItem.RemoveRange(cartItems); // Clear the selected items from the cart
            _db.SaveChanges();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            // Log the exception (you can use a logging framework)
            var errorMessage = $"Error creating order: {ex.Message}";
            if (ex.InnerException != null)
            {
                errorMessage += $"; Inner exception: {ex.InnerException.Message}";
            }
            Console.WriteLine(errorMessage);

            return Json(new { success = false, message = errorMessage });
        }
    }

    [HttpPost]
    public IActionResult DeleteOrder(int orderId)
    {
        try
        {
            var order = _db.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found" });
            }

            _db.Orders.Remove(order);
            _db.SaveChanges();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            var errorMessage = $"Error deleting order: {ex.Message}";
            if (ex.InnerException != null)
            {
                errorMessage += $"; Inner exception: {ex.InnerException.Message}";
            }
            Console.WriteLine(errorMessage);

            return Json(new { success = false, message = errorMessage });
        }
    }

    [HttpPost]
    public IActionResult Checkout(int orderId)
    {
        try
        {
            // Find the order based on orderId
            var order = _db.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                                  .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                return Json(new { success = false, message = "Order not found." });
            }

            // Assuming some logic to update order status, etc.
            // Here you can update the order's status or perform other necessary operations

            // Redirect to PaymentPage with orderId
            return Json(new { success = true, redirectUrl = Url.Action("PaymentPage", new { orderId }) });
        }
        catch (Exception ex)
        {
            var errorMessage = $"Error during checkout: {ex.Message}";
            if (ex.InnerException != null)
            {
                errorMessage += $"; Inner exception: {ex.InnerException.Message}";
            }
            Console.WriteLine(errorMessage);

            return Json(new { success = false, message = errorMessage });
        }
    }
    public IActionResult ThankYou(int orderId)
    {
        try
        {
            // Retrieve the order based on orderId
            var order = _db.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                                  .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            // Assuming you have a way to get the user's name (e.g., from a user table or session)
            // Replace this with actual logic to retrieve user details
            string userName = "Akram"; // Replace with actual user retrieval logic

            var viewModel = new ThankYouViewModel
            {
                Order = order,
                UserName = userName
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            var errorMessage = $"Error retrieving order details: {ex.Message}";
            if (ex.InnerException != null)
            {
                errorMessage += $"; Inner exception: {ex.InnerException.Message}";
            }
            Console.WriteLine(errorMessage);

            return StatusCode(500, "Internal server error");
        }
    }

    public IActionResult PaymentPage(int orderId)
    {
        // Retrieve the order based on orderId
        var order = _db.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                              .FirstOrDefault(o => o.OrderId == orderId);
        if (order == null)
        {
            return NotFound();
        }
        return View(order);
    }
}
