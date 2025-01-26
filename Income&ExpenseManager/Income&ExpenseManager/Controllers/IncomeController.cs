using Income_ExpenseManager.BAL;
using Income_ExpenseManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace Income_ExpenseManager.Controllers
{
    public class IncomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string baseUrl = "https://localhost:7291/api/Income";

        public IncomeController(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        #region AddForm
        public async Task<IActionResult> AddForm(int? incomeId)
        {
            await SetCategoriesDropdown();
            ViewBag.UserId = CV.UserId();

            if (incomeId != null)
            {
                var response = await _httpClient.GetAsync($"https://localhost:7291/api/Income/SelectByIncomeID/{incomeId}");

                if (response.IsSuccessStatusCode)
                {
                    var incomeData = await response.Content.ReadAsStringAsync();    
                    IncomeModel model = JsonConvert.DeserializeObject<IncomeModel>(incomeData);
                    //List<setCategoriesDropDown> categories = await SetCategoriesDropdown();

                    //List<SelectListItem> selectListItems = new List<SelectListItem>();
                    //foreach (setCategoriesDropDown models in categories)
                    //{
                    //    selectListItems.Add(new SelectListItem { Value = models.CategoryId.ToString(), Text = models.CategoryName});
                    //}
                    //ViewBag.StateList = selectListItems;
                    return View(model);
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddForm(IncomeModel incomeModel)
        {
            await SetCategoriesDropdown();
            ViewBag.UserId = CV.UserId();

            if (ModelState.IsValid)
            {
                incomeModel.UserID = CV.UserId();
                var json = JsonConvert.SerializeObject(incomeModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (incomeModel.IncomeID > 0)
                    response = await _httpClient.PutAsync($"{baseUrl}/updateIncome", content);
                else
                    response = await _httpClient.PostAsync($"{baseUrl}/InsertIncome", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = incomeModel.IncomeID > 0
                        ? "Income updated successfully!"
                        : "Income added successfully!";
                    return RedirectToAction("GetIncomeData");
                }

                ModelState.AddModelError("", "Failed to save income data. Please try again.");
            }

            return View(incomeModel);
        }
        #endregion

        #region GetIncomeData
        public async Task<IActionResult> GetIncomeData()
        {
            var userId = CV.UserId();
            var response = await _httpClient.GetAsync($"{baseUrl}/SelectByUserID/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to fetch income data.";
                return View(new List<IncomeModel>());
            }

            var data = await response.Content.ReadAsStringAsync();
            var incomes = JsonConvert.DeserializeObject<List<IncomeModel>>(data);
            return View(incomes);
        }
        #endregion

        #region DeleteIncomeData
        public async Task<IActionResult> DeleteIncomeData(int incomeId)
        {
            var response = await _httpClient.DeleteAsync($"{baseUrl}/Delete/{incomeId}");

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Income deleted successfully!";
            else
                TempData["Error"] = "Failed to delete income.";

            return RedirectToAction("GetIncomeData");
        }
        #endregion

        #region Helper Methods
        private async Task SetCategoriesDropdown()
        {
            var response = await _httpClient.GetAsync("https://localhost:7291/api/Category/income");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
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
        #endregion
    }
}
