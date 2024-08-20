using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
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

            int userId = 2; // Replace with the actual logged-in user ID

            var cartItems = _db.CartItem.Include(c => c.Product)
                                        .Where(c => selectedItems.Contains(c.CartId) && c.UserID == userId)
                                        .ToList();
            if (!cartItems.Any())
            {
                return Json(new { success = false, message = "No items in cart to order" });
            }

            int orderNumber = _db.GetNextOrderNumber();

            var order = new Order
            {
                OrderNumber = orderNumber,
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
            _db.CartItem.RemoveRange(cartItems);
            _db.SaveChanges();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
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
            var order = _db.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                                  .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                return Json(new { success = false, message = "Order not found." });
            }

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
            var order = _db.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                                  .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            string userName = "Sadiq"; // Replace with actual user retrieval logic

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
        var order = _db.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                              .FirstOrDefault(o => o.OrderId == orderId);
        if (order == null)
        {
            return NotFound();
        }
        return View(order);
    }

    public IActionResult InvoiceList(int orderId)
    {
        var order = _db.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                              .FirstOrDefault(o => o.OrderId == orderId);

        if (order == null)
        {
            return NotFound();
        }

        var viewModel = new Invoice
        {
            InvoiceId = GenerateInvoiceId(), // Replace with your logic to generate invoice ID
            Order = order,
            UserName = "Sadiq Ahmed Khan", // Replace with logic to get the actual user's name
            UserAddress = "123 Main St, Kurnool, India", // Replace with logic to get the actual user's address
            MobileNumber = "123456789",
            EmailId = "sadiqkhantom@gmail.com",
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> DownloadInvoice(int orderId)
    {
        try
        {
            // Fetch order data from the database
            var order = await _db.Orders.Include(o => o.OrderItems)
                                        .ThenInclude(oi => oi.Product)
                                        .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            var viewModel = new Invoice
            {
                InvoiceId = GenerateInvoiceId(),
                Order = order,
                UserName = "Sadiq Ahmed Khan",
                UserAddress = "123 Main St, Kurnool, India",
                MobileNumber = "123456789",
                EmailId = "sadiqkhantom@gmail.com"
            };

            using (var ms = new MemoryStream())
            {
                using (var document = new PdfDocument())
                {
                    var page = document.AddPage();
                    page.Width = XUnit.FromPoint(700); // Set page width
                    page.Height = XUnit.FromPoint(1000); // Set page height
                    using (var gfx = XGraphics.FromPdfPage(page))
                    {
                        var font = new XFont("Arial", 12, XFontStyle.Regular);
                        var boldFont = new XFont("Arial", 12, XFontStyle.Bold);
                        var titleFont = new XFont("Arial", 24, XFontStyle.Bold);
                        var smallFont = new XFont("Arial", 10, XFontStyle.Regular);

                        // Define positions for text
                        int xMargin = 40;
                        int yMargin = 40;
                        int lineHeight = 20;

                        // Add invoice title centered
                        gfx.DrawString("Invoice", titleFont, XBrushes.DarkRed,
                            new XRect(0, yMargin + 40, page.Width, 0), XStringFormats.TopCenter);

                        // Add company logo below the invoice title
                        string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "css", "logo.png");
                        if (System.IO.File.Exists(logoPath))
                        {
                            XImage logo = XImage.FromFile(logoPath);
                            gfx.DrawImage(logo, xMargin, yMargin + 80, 150, 50); // Adjust size and position as needed
                        }
                        else
                        {
                            gfx.DrawString("Logo not found", font, XBrushes.Red,
                                new XRect(xMargin, yMargin + 80, page.Width, 0), XStringFormats.TopLeft);
                        }

                        // Add user details on the right side parallel to the logo
                        gfx.DrawString($"Name: {viewModel.UserName}", font, XBrushes.Black,
                            new XRect(page.Width - 400, yMargin + 80, 300, 0), XStringFormats.TopRight);
                        gfx.DrawString($"Address: {viewModel.UserAddress}", font, XBrushes.Black,
                            new XRect(page.Width - 400, yMargin + 100, 300, 0), XStringFormats.TopRight);
                        gfx.DrawString($"Mobile: {viewModel.MobileNumber}", font, XBrushes.Black,
                            new XRect(page.Width - 400, yMargin + 120, 300, 0), XStringFormats.TopRight);
                        gfx.DrawString($"Email: {viewModel.EmailId}", font, XBrushes.Black,
                            new XRect(page.Width - 400, yMargin + 140, 300, 0), XStringFormats.TopRight);

                        // Add a horizontal line below the logo and user details
                        gfx.DrawLine(XPens.Black, xMargin, yMargin + 180, page.Width - xMargin, yMargin + 180);

                        // Add table header with bold font
                        int yPosition = yMargin + 200;
                        gfx.DrawString("Invoice ID", boldFont, XBrushes.Brown,
                            new XRect(xMargin, yPosition, page.Width, 0), XStringFormats.TopLeft);
                        gfx.DrawString("Order ID", boldFont, XBrushes.Brown,
                            new XRect(xMargin + 100, yPosition, page.Width, 0), XStringFormats.TopLeft);
                        gfx.DrawString("Product Name", boldFont, XBrushes.Brown,
                            new XRect(xMargin + 200, yPosition, page.Width, 0), XStringFormats.TopLeft);
                        gfx.DrawString("Quantity", boldFont, XBrushes.Brown,
                            new XRect(xMargin + 350, yPosition, page.Width, 0), XStringFormats.TopLeft);
                        gfx.DrawString("Price", boldFont, XBrushes.Brown,
                            new XRect(xMargin + 450, yPosition, page.Width, 0), XStringFormats.TopLeft);
                        gfx.DrawString("Total Price", boldFont, XBrushes.Brown,
                            new XRect(xMargin + 550, yPosition, page.Width, 0), XStringFormats.TopLeft);

                        // Add horizontal line below the table header
                        gfx.DrawLine(XPens.Black, xMargin, yPosition + lineHeight, page.Width - xMargin, yPosition + lineHeight);


                        yPosition += lineHeight * 2;

                        // Add table rows
                        foreach (var item in viewModel.Order.OrderItems)
                        {
                            gfx.DrawString(viewModel.InvoiceId.ToString(), font, XBrushes.Gold,
                                new XRect(xMargin, yPosition, page.Width, 0), XStringFormats.TopLeft);
                            gfx.DrawString(item.OrderId.ToString(), font, XBrushes.Goldenrod,
                                new XRect(xMargin + 100, yPosition, page.Width, 0), XStringFormats.TopLeft);
                            gfx.DrawString(item.Product.ProductName, font, XBrushes.DarkBlue,
                                new XRect(xMargin + 200, yPosition, page.Width, 0), XStringFormats.TopLeft);
                            gfx.DrawString(item.Quantity.ToString(), font, XBrushes.DarkBlue,
                                new XRect(xMargin + 350, yPosition, page.Width, 0), XStringFormats.TopLeft);
                            gfx.DrawString($"₹ {item.Price.ToString("N2")}", font, XBrushes.DarkBlue,
                                new XRect(xMargin + 450, yPosition, page.Width, 0), XStringFormats.TopLeft);
                            gfx.DrawString($"₹ {(item.Quantity * item.Price).ToString("N2")}", font, XBrushes.LimeGreen,
                                new XRect(xMargin + 550, yPosition, page.Width, 0), XStringFormats.TopLeft);


                            yPosition += lineHeight;
                        }

                        // Add total price aligned to the right side
                        gfx.DrawString($"Grand Total: ₹ {viewModel.Order.TotalPrice.ToString("N2")}", boldFont, XBrushes.ForestGreen,
                            new XRect(page.Width - 400, yPosition + 20, 300, 0), XStringFormats.TopRight);
                    }

                    document.Save(ms);
                }

                return File(ms.ToArray(), "application/pdf", $"Invoice_{viewModel.InvoiceId}.pdf");
            }
        }
        catch (Exception ex)
        {
            var errorMessage = $"Error generating PDF: {ex.Message}";
            if (ex.InnerException != null)
            {
                errorMessage += $"; Inner exception: {ex.InnerException.Message}";
            }
            Console.WriteLine(errorMessage); // Or use a proper logging mechanism
            return StatusCode(500, $"Internal server error: {errorMessage}");
        }
    }

    private int GenerateInvoiceId()
    {
        return new Random().Next(1000, 9999); // Replace with a proper ID generation logic
    }
}
