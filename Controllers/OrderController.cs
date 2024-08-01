using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UPC_DropDown.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
    public IActionResult Checkout()
    {
        try
        {
            // Your order checkout logic here
            // This could include updating order status, processing payment, etc.

            return Json(new { success = true });
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

    public IActionResult ThankYou()
    {
        return View(ThankYou);
    }
}
