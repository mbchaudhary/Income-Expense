using Microsoft.AspNetCore.Mvc;

namespace Income_ExpenseManager.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult GetCategoryData()
        {
            return View();
        }

        public IActionResult AddForm()
        {
            return View();
        }
    }
}
