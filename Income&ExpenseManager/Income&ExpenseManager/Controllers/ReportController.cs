using ClosedXML.Excel;
using Income_ExpenseManager.BAL;
using Income_ExpenseManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

public class ReportController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly string baseUrl = "https://localhost:7291/api/Report";

    public ReportController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IActionResult> GetReportData(DateTime? startDate, DateTime? endDate, string? type, string? category, decimal? minAmount, decimal? maxAmount)
    {
        await SetCategoriesDropdown();

        try
        {
            var userId = CV.UserId();
            var response = await _httpClient.GetAsync($"{baseUrl}/IncomeAndExpenseReportByUserID?UserID={userId}");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Failed to retrieve report data.";
                return View(new List<ReportModel>());
            }

            var data = await response.Content.ReadAsStringAsync();
            var reportList = JsonConvert.DeserializeObject<List<ReportModel>>(data) ?? new List<ReportModel>();

            // Apply LINQ filtering
            var filteredReports = reportList.AsEnumerable();

            if (startDate.HasValue)
                filteredReports = filteredReports.Where(r => r.Date >= startDate.Value);
            if (endDate.HasValue)
                filteredReports = filteredReports.Where(r => r.Date <= endDate.Value);
            if (!string.IsNullOrEmpty(type))
                filteredReports = filteredReports.Where(r => r.Type?.Equals(type, StringComparison.OrdinalIgnoreCase) == true);
            if (!string.IsNullOrEmpty(category))
                filteredReports = filteredReports.Where(r => r.Category?.Equals(category, StringComparison.OrdinalIgnoreCase) == true);
            if (minAmount.HasValue)
                filteredReports = filteredReports.Where(r => r.Amount >= minAmount.Value);
            if (maxAmount.HasValue)
                filteredReports = filteredReports.Where(r => r.Amount <= maxAmount.Value);

            return View(filteredReports.ToList());
        }
        catch (Exception)
        {
            ViewBag.ErrorMessage = "An error occurred while fetching the report data.";
            return View(new List<ReportModel>());
        }
    }


    public async Task<IActionResult> ExportToExcel()
    {
        var userId = CV.UserId();
        var response = await _httpClient.GetAsync($"{baseUrl}/IncomeAndExpenseReportByUserID?UserID={userId}");

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "Failed to fetch report data.";
            return RedirectToAction("GetReportData");
        }

        var data = await response.Content.ReadAsStringAsync();
        var reports = JsonConvert.DeserializeObject<List<ReportModel>>(data);

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Expense Report");

            // Add Headers
            worksheet.Cell(1, 1).Value = "No.";
            worksheet.Cell(1, 2).Value = "Amount";
            worksheet.Cell(1, 3).Value = "Category";
            worksheet.Cell(1, 4).Value = "Date";
            worksheet.Cell(1, 5).Value = "Notes";
            worksheet.Cell(1, 5).Value = "Type";

            // Add Data
            int row = 2;
            int index = 1;
            foreach (var r in reports)
            {
                worksheet.Cell(row, 1).Value = index++;
                worksheet.Cell(row, 2).Value = r.Amount;
                worksheet.Cell(row, 3).Value = r.Category;
                worksheet.Cell(row, 4).Value = r.Date.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 5).Value = r.Notes;
                worksheet.Cell(row, 5).Value = r.Type;
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Report.xlsx");
            }
        }
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
    }
}
