using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UPC_DropDown.Models;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _db;

    public ProductController(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<IActionResult> ProductsList(int pageNumber = 1, int pageSize = 10)
    {
        if (pageSize <= 0)
        {
            pageSize = 10; // default page size
        }

        var totalCount = await _db.Products.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        if (pageNumber < 1)
        {
            pageNumber = 1;
        }
        else if (pageNumber > totalPages)
        {
            pageNumber = totalPages;
        }

        var items = await _db.Products
            .OrderBy(x => x.ProductID)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewData["PageNumber"] = pageNumber;
        ViewData["PageSize"] = pageSize;
        ViewData["TotalPages"] = totalPages;

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_ProductListPartial", items);
        }

        return View(items);
    }


    [HttpPost]
    public async Task<IActionResult> SearchProducts([FromBody] string keyword, int pageNumber = 1, int pageSize = 10)
    {
        var products = await GetProductsAsync(keyword, pageNumber, pageSize);
        var totalCount = await _db.Products.CountAsync(p => p.ProductName.Contains(keyword));
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        ViewData["PageNumber"] = pageNumber;
        ViewData["PageSize"] = pageSize;
        ViewData["TotalPages"] = totalPages;

        return PartialView("_ProductListPartial", products);
    }

    private async Task<List<Product>> GetProductsAsync(string keyword, int pageNumber, int pageSize)
    {
        if (string.IsNullOrEmpty(keyword))
        {
            return await _db.Products
                .OrderBy(x => x.ProductID)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        else
        {
            return await _db.Products
                .Where(p => p.ProductName.Contains(keyword))
                .OrderBy(x => x.ProductID)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }

    public IActionResult AddProduct()
    {
        var categories = _db.Categories.ToList();
        var model = new AddProductModel
        {
            Categories = categories.Select(c => new SelectListItem { Text = c.CategoryName, Value = c.CategoryID.ToString() })
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult SaveProduct(AddProductModel model)
    {
        if (ModelState.IsValid)
        {
            var product = new Product
            {
                ProductName = model.ProductName,
                ProductPrice = model.ProductPrice,
                ProductDescription = model.ProductDescription,
                CategoryID = model.CategoryID.GetValueOrDefault(),
                Category = _db.Categories.Find(model.CategoryID)
            };

            _db.Products.Add(product);
            _db.SaveChanges();

            return RedirectToAction("ProductsList");
        }

        model.Categories = _db.Categories.Select(c => new SelectListItem { Text = c.CategoryName, Value = c.CategoryID.ToString() });
        return View("AddProduct", model);
    }

    public IActionResult EditProduct(int ProductID)
    {
        var categories = _db.Categories.ToList();
        var productObj = _db.Products.FirstOrDefault(p => p.ProductID == ProductID);

        var model = new EditProductModel
        {
            ProductID = productObj.ProductID,
            ProductName = productObj.ProductName,
            ProductPrice = productObj.ProductPrice,
            ProductDescription = productObj.ProductDescription,
            CategoryID = productObj.CategoryID,
            Categories = categories.Select(c => new SelectListItem { Text = c.CategoryName, Value = c.CategoryID.ToString() })
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult UpdateProduct(EditProductModel model)
    {
        var productObj = _db.Products.FirstOrDefault(p => p.ProductID == model.ProductID);
        if (productObj != null)
        {
            productObj.ProductName = model.ProductName;
            productObj.ProductPrice = model.ProductPrice;
            productObj.ProductDescription = model.ProductDescription;
            productObj.CategoryID = model.CategoryID;

            _db.SaveChanges();
        }

        return RedirectToAction("ProductsList");
    }

    [HttpPost]
    public IActionResult DeleteProduct(int productId)
    {
        try
        {
            var productObj = _db.Products.FirstOrDefault(p => p.ProductID == productId);

            if (productObj == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            _db.Products.Remove(productObj);
            _db.SaveChanges();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    public async Task<IActionResult> ExportToExcel()
    {
        var products = await _db.Products.Include(p => p.Category).ToListAsync();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Products");
            var currentRow = 1;

            worksheet.Cell(currentRow, 1).Value = "Product ID";
            worksheet.Cell(currentRow, 2).Value = "Product Name";
            worksheet.Cell(currentRow, 3).Value = "Price";
            worksheet.Cell(currentRow, 4).Value = "Description";
            worksheet.Cell(currentRow, 5).Value = "Category Name";

            foreach (var product in products)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = product.ProductID;
                worksheet.Cell(currentRow, 2).Value = product.ProductName;
                worksheet.Cell(currentRow, 3).Value = product.ProductPrice;
                worksheet.Cell(currentRow, 4).Value = product.ProductDescription;
                worksheet.Cell(currentRow, 5).Value = product.Category?.CategoryName;
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Products.xlsx");
            }
        }
    }
}
