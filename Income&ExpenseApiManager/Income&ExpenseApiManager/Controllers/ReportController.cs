using Income_ExpenseApiManager.Data;
using Income_ExpenseApiManager.Model;
using Microsoft.AspNetCore.Mvc;

namespace Income_ExpenseApiManager.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        [HttpGet]
        public IActionResult IncomeAndExpenseReportByUserID(int UserID, ReportRepositery reportRepositery)
        {
            if (UserID <= 0)
            {
                return BadRequest();
            }

            List<ReportModel> model = reportRepositery.IncomeAndExpenseReportByUserId(UserID);

            if (model == null || model.Count <= 0)
            {
                return BadRequest();
            }

            return Ok(model);
        }
    }
}
