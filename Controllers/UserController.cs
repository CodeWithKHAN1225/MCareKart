using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UPC_DropDown.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class UserController : Controller
{
    private readonly ApplicationDbContext _db;

    public UserController(ApplicationDbContext db)
    {
        _db = db;
    }

    // Method to display the list of users
    public async Task<IActionResult> UsersList()
    {
        var users = await GetUsersAsync(string.Empty);
        return View(users);
    }

    // Method to search users with a keyword
    [HttpPost]
    public async Task<IActionResult> SearchUsers([FromBody] string keyword)
    {
        var users = await GetUsersAsync(keyword);
        return PartialView("_UserListPartial", users);
    }

    // Helper method to retrieve users from the database
    private async Task<List<User>> GetUsersAsync(string keyword)
    {
        if (string.IsNullOrEmpty(keyword))
        {
            return await _db.Users.FromSqlRaw("EXEC GetUsers").ToListAsync();
        }
        else
        {
            SqlParameter param = new SqlParameter("@p0", keyword);
            return await _db.Users.FromSqlRaw("EXEC SearchUsers @p0", param).ToListAsync();
        }
    }

    // Method to export users data to Excel
    public async Task<IActionResult> ExportToExcel()
    {
        var users = await _db.Users.ToListAsync();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Users");
            var currentRow = 1;

            // Header row
            worksheet.Cell(currentRow, 1).Value = "User ID";
            worksheet.Cell(currentRow, 2).Value = "User Name";
            worksheet.Cell(currentRow, 3).Value = "User Password";
            worksheet.Cell(currentRow, 4).Value = "User Email";
            worksheet.Cell(currentRow, 5).Value = "Salary";

            // Data rows
            foreach (var user in users)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = user.UserID;
                worksheet.Cell(currentRow, 2).Value = user.UserName;
                worksheet.Cell(currentRow, 3).Value = user.UserPassword;
                worksheet.Cell(currentRow, 4).Value = user.UserEmail;
                worksheet.Cell(currentRow, 5).Value = user.Salary;
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Users.xlsx");
            }
        }
    }
}
