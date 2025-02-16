using Income_ExpenseApiManager.Data;
using Income_ExpenseApiManager.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Income_ExpenseApiManager.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        #region configuration
        public ExpenseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        [HttpGet("{id}")]

        public IActionResult GetExpenseDataByUserID(int id, ExpenseRepositery expenseRepositery)
        {
            if(id<=0)
            {
                return BadRequest();
            }

            List<ExpenseModel> model = expenseRepositery.SelectByUserID(id);

            if(model == null || model.Count<=0)
            {
                return BadRequest();
            }
            
            return Ok(model);
        }

        [HttpGet("{id}")]

        public IActionResult SelectByExpenseID(int id, ExpenseRepositery expenseRepositery)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid User ID");
            }

            ExpenseModel expenses = expenseRepositery.SelectByExpenseID(id);

            if (expenses == null)
            {
                return NotFound("No expense records found for the given User ID.");
            }

            return Ok(expenses);
        }


        [HttpDelete("{id}")]

        public IActionResult DeleteExpense(int id)
        {
            string cs = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection conn = new SqlConnection(cs);

            conn.Open();

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "DeleteExpense";
            cmd.Parameters.AddWithValue("@ExpenseId", id);
            int rowAffected = cmd.ExecuteNonQuery();

            conn.Close();

            if(rowAffected > 0)
            {
                return Ok("SuccessFully Delete");
            }
            else
            {
                return BadRequest();
            }
        }


        [HttpPost]

        public IActionResult InsertExpense([FromBody] ExpenseModel model, ExpenseRepositery expenseRepositery)
        {
            if (model == null)
            {
                return BadRequest();
            }

            bool isInserted = expenseRepositery.InsertExpense(model);

            if (isInserted)
            {
                return Ok("Insert Successful");
            }
            else
            {
                return BadRequest();
            }
        }


        [HttpPut]

        public IActionResult ExpenseUpdate([FromBody] ExpenseModel model, ExpenseRepositery expenseRepositery)
        {
            if (model == null)
            {
                return BadRequest();
            }

            bool isUpdated = expenseRepositery.UpdateExpense(model);

            if (isUpdated)
            {
                return Ok("Successfully Updated");
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
