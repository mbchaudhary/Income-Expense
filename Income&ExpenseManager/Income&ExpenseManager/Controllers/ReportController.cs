using Microsoft.AspNetCore.Mvc;

namespace Income_ExpenseManager.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult GetReportData()
        {
            return View();
        }
    }
}
