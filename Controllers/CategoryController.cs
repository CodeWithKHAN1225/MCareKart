using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UPC_DropDown.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClosedXML.Excel;

public class CategoryController : Controller
{
    private readonly ApplicationDbContext _db;

    public CategoryController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> CategoriesList()
    {
        var categories = await GetCategoriesAsync(string.Empty);
        return View(categories);
    }

    [HttpPost]
    public async Task<IActionResult> SearchCategories([FromBody] string keyword)
    {
        var categories = await GetCategoriesAsync(keyword);
        return PartialView("_CategoryListPartial", categories);
    }

    private async Task<List<Category>> GetCategoriesAsync(string keyword)
    {
        if (string.IsNullOrEmpty(keyword))
        {
            return await _db.Categories.FromSqlRaw("EXEC GetCategories").ToListAsync();
        }
        else
        {
            SqlParameter param = new SqlParameter("@p0", keyword);
            return await _db.Categories.FromSqlRaw("EXEC SearchCategories @p0", param).ToListAsync();
        }
    }
    public async Task<IActionResult> ExportToExcel()
    {
        var categories = await _db.Categories.ToListAsync();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Categories");
            var currentRow = 1;

            worksheet.Cell(currentRow, 1).Value = "Category ID";
            worksheet.Cell(currentRow, 2).Value = "Category Name";
            worksheet.Cell(currentRow, 3).Value = "Description";
            worksheet.Cell(currentRow, 4).Value = "Status";

            foreach (var category in categories)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = category.CategoryID;
                worksheet.Cell(currentRow, 2).Value = category.CategoryName;
                worksheet.Cell(currentRow, 3).Value = category.CategoryDescription;
                worksheet.Cell(currentRow, 4).Value = category.CategoryStatus;
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Categories.xlsx");
            }
        }
    }
}
