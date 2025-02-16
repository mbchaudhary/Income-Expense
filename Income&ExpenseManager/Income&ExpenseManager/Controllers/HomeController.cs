using Income_ExpenseManager.BAL;
using Income_ExpenseManager.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class HomeController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HomeController> _logger;

    public HomeController(HttpClient httpClient, ILogger<HomeController> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IActionResult> Home()
    {
        int userId = CV.UserId();
        decimal totalIncome = 0, totalExpense = 0, netSavings = 0;
        List<TransactionModel> transactions = new List<TransactionModel>();
        List<int> monthlyIncome = new List<int>(new int[12]);
        List<int> monthlyExpense = new List<int>(new int[12]);

        try
        {
            // Fetch Total Income
            var incomeResponse = await _httpClient.GetAsync($"https://localhost:7291/api/Income/SelectByUserID/{userId}");
            if (incomeResponse.IsSuccessStatusCode)
            {
                var incomeData = await incomeResponse.Content.ReadAsStringAsync();
                var incomes = JsonConvert.DeserializeObject<List<IncomeModel>>(incomeData) ?? new List<IncomeModel>();
                totalIncome = incomes.Sum(i => i.IncomeAmount);

                foreach (var income in incomes)
                {
                    int monthIndex = income.IncomeDate.Month - 1;
                    monthlyIncome[monthIndex] += (int)income.IncomeAmount;

                    transactions.Add(new TransactionModel
                    {
                        Date = income.IncomeDate,
                        Description = income.Notes,
                        Category = income.IncomeSource,
                        Amount = income.IncomeAmount
                    });
                }
            }

            // Fetch Total Expenses
            var expenseResponse = await _httpClient.GetAsync($"https://localhost:7291/api/Expense/GetExpenseDataByUserID/{userId}");
            if (expenseResponse.IsSuccessStatusCode)
            {
                var expenseData = await expenseResponse.Content.ReadAsStringAsync();
                var expenses = JsonConvert.DeserializeObject<List<ExpenseModel>>(expenseData) ?? new List<ExpenseModel>();
                totalExpense = expenses.Sum(e => e.ExpenseAmount);

                foreach (var expense in expenses)
                {
                    int monthIndex = expense.ExpenseDate.Month - 1;
                    monthlyExpense[monthIndex] += (int)expense.ExpenseAmount;

                    transactions.Add(new TransactionModel
                    {
                        Date = expense.ExpenseDate,
                        Description = expense.Notes,
                        Category = expense.ExpenseCategory,
                        Amount = -expense.ExpenseAmount
                    });
                }
            }

            // Calculate Net Savings
            netSavings = totalIncome - totalExpense;

            // Sort transactions by date (latest first) and take only the top 3
            transactions = transactions.OrderByDescending(t => t.Date).Take(3).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching data: {ex.Message}");
            ViewData["ErrorMessage"] = "Error retrieving dashboard data. Please try again later.";
        }

        // Pass data to View
        ViewData["TotalIncome"] = totalIncome;
        ViewData["TotalExpense"] = totalExpense;
        ViewData["NetSavings"] = netSavings;
        ViewData["Transactions"] = transactions;
        ViewData["MonthlyIncome"] = monthlyIncome;
        ViewData["MonthlyExpense"] = monthlyExpense;

        return View();
    }
}
