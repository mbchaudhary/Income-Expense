using ClosedXML.Excel;
using Income_ExpenseManager.BAL;
using Income_ExpenseManager.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Income_ExpenseManager.Controllers
{
    public class CategoryController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string baseUrl = "https://localhost:7291/api/Category";

        public CategoryController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IActionResult> GetCategoryData()
        {
            var userId = CV.UserId();
            var res = await _httpClient.GetAsync($"{baseUrl}/GetExpenseDataByUserID/{userId}");

            if(!res.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to fetch Category data.";
                return View(new List<CategoriesModel>());
            }

            var data = await res.Content.ReadAsStringAsync();
            var category = JsonConvert.DeserializeObject<List<CategoriesModel>>(data);

            return View(category);
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var userId = CV.UserId();
            var response = await _httpClient.GetAsync($"{baseUrl}/GetExpenseDataByUserID/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to fetch Categories data.";
                return RedirectToAction("GetCategoryData");
            }

            var data = await response.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<List<CategoriesModel>>(data);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Categories Report");

                // Add Headers
                worksheet.Cell(1, 1).Value = "No.";
                worksheet.Cell(1, 2).Value = "Category Name";
                worksheet.Cell(1, 3).Value = "Category Type";

                // Add Data
                int row = 2;
                int index = 1;
                foreach (var c in categories)
                {
                    worksheet.Cell(row, 1).Value = index++;
                    worksheet.Cell(row, 2).Value = c.CategoryName;
                    worksheet.Cell(row, 3).Value = c.CategoryType;
                    row++;
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CategoriesReport.xlsx");
                }
            }
        }

        public async Task<IActionResult> AddForm(int? categoryId)
        {
            ViewBag.UserId = CV.UserId();

            if(categoryId != null)
            {
                var res = await _httpClient.GetAsync($"{baseUrl}/SelectByCategoryID/{categoryId}");

                if(res.IsSuccessStatusCode)
                {
                    var categoryData = await res.Content.ReadAsStringAsync();
                    CategoriesModel model = JsonConvert.DeserializeObject<CategoriesModel>(categoryData);
                    return View(model);
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddForm(CategoriesModel categories)
        {
            ViewBag.UserId = CV.UserId();

            if (ModelState.IsValid)
            {
                categories.UserId = CV.UserId();
                var json = JsonConvert.SerializeObject(categories);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (categories.CategoryId > 0 && categories.CategoryId != null)
                    response = await _httpClient.PutAsync($"{baseUrl}/CategoriesUpdate", content);
                else
                    response = await _httpClient.PostAsync($"{baseUrl}/InsertCategory", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = categories.CategoryId > 0
                        ? "Category updated successfully!"
                        : "Category added successfully!";
                    return RedirectToAction("GetCategoryData");
                }

                ModelState.AddModelError("", "Failed to save Category data. Please try again.");
            }

            return View(categories);
        }


        public async Task<IActionResult> DeleteCategoryData(int categoryId)
        {
            var res = await _httpClient.DeleteAsync($"https://localhost:7291/api/Category/DeletCategoty/{categoryId}");

            TempData["SuccessMessage"] = res.IsSuccessStatusCode ? "Successfully Deleted!" : "Failed to delete Categories.";
            return RedirectToAction("GetCategoryData");
        }
    }
}
