using Microsoft.AspNetCore.Mvc;

namespace Income_ExpenseManager.Controllers
{
    public class ExpenseController : Controller
    {
        public IActionResult AddForm()
        {
            return View();
        }

        public IActionResult GetExpenseData()
        {
            return View();
        }
    }
}
