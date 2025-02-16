using ClosedXML.Excel;
using Income_ExpenseManager.BAL;
using Income_ExpenseManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

namespace Income_ExpenseManager.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string baseUrl = "https://localhost:7291/api/Expense";

        public ExpenseController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> AddForm(int? expenseId)
        {
            await SetCategoriesDropdown();
            ViewBag.UserId = CV.UserId();

            if (expenseId != null)
            {
                var res = await _httpClient.GetAsync($"{baseUrl}/SelectByExpenseID/{expenseId}");

                if (res.IsSuccessStatusCode)
                {
                    var expenseData = await res.Content.ReadAsStringAsync();
                    ExpenseModel model = JsonConvert.DeserializeObject<ExpenseModel>(expenseData);
                    return View(model);
                }
            }

            return View();
        }


        public async Task<IActionResult> ExportToExcel()
        {
            var userId = CV.UserId();
            var response = await _httpClient.GetAsync($"{baseUrl}/GetExpenseDataByUserID/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to fetch expense data.";
                return RedirectToAction("GetExpenseData");
            }

            var data = await response.Content.ReadAsStringAsync();
            var expenses = JsonConvert.DeserializeObject<List<ExpenseModel>>(data);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Expense Report");

                // Add Headers
                worksheet.Cell(1, 1).Value = "No.";
                worksheet.Cell(1, 2).Value = "Expense Amount";
                worksheet.Cell(1, 3).Value = "Expense Source";
                worksheet.Cell(1, 4).Value = "Expense Date";
                worksheet.Cell(1, 5).Value = "Notes";

                // Add Data
                int row = 2;
                int index = 1;
                foreach (var expense in expenses)
                {
                    worksheet.Cell(row, 1).Value = index++;
                    worksheet.Cell(row, 2).Value = expense.ExpenseAmount;
                    worksheet.Cell(row, 3).Value = expense.ExpenseCategory;
                    worksheet.Cell(row, 4).Value = expense.ExpenseDate.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 5).Value = expense.Notes;
                    row++;
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExpenseReport.xlsx");
                }
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddForm(ExpenseModel expenseModel)
        {
            await SetCategoriesDropdown();
            ViewBag.UserId = CV.UserId();

            if (ModelState.IsValid)
            {
                expenseModel.UserId = CV.UserId();
                var json = JsonConvert.SerializeObject(expenseModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (expenseModel.ExpenseId > 0 && expenseModel.ExpenseId != null)
                    response = await _httpClient.PutAsync($"{baseUrl}/ExpenseUpdate", content);
                else
                    response = await _httpClient.PostAsync($"{baseUrl}/InsertExpense", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = expenseModel.ExpenseId > 0
                        ? "Expense updated successfully!"
                        : "Expense added successfully!";
                    return RedirectToAction("GetExpenseData");
                }

                ModelState.AddModelError("", "Failed to save Expense data. Please try again.");
            }

            return View(expenseModel);
        }

        public async Task<IActionResult> GetExpenseData()
        {
            var userId = CV.UserId();
            var response = await _httpClient.GetAsync($"{baseUrl}/GetExpenseDataByUserID/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to fetch Expense data.";
                return View(new List<ExpenseModel>());
            }

            var data = await response.Content.ReadAsStringAsync();
            var expense = JsonConvert.DeserializeObject<List<ExpenseModel>>(data);
            return View(expense);
        }

        public async Task<IActionResult> DeleteExpenseData(int expenseId)
        {
            var res = await _httpClient.DeleteAsync($"{baseUrl}/DeleteExpense/{expenseId}");

            TempData["SuccessMessage"] = res.IsSuccessStatusCode ? "Successfully Deleted!" : "Failed to delete Expense.";
            return RedirectToAction("GetExpenseData");
        }

        private async Task SetCategoriesDropdown()
        {
            var res = await _httpClient.GetAsync("https://localhost:7291/api/Category/GetAllCategory");

            if (res.IsSuccessStatusCode)
            {
                var data = await res.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<CategoriesModel>>(data);

                ViewBag.CategoriesList = categories?.Select(c => new SelectListItem
                {
                    Text = c.CategoryName,
                    Value = c.CategoryId.ToString()
                }).ToList() ?? new List<SelectListItem>();
            }
            else
            {
                ViewBag.CategoriesList = new List<SelectListItem>();
                TempData["Error"] = "Failed to fetch categories.";
            }
        }
    }
}
