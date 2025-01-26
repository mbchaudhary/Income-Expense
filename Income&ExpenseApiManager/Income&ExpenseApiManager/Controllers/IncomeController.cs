using Income_ExpenseApiManager.Data;
using Income_ExpenseApiManager.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Income_ExpenseApiManager.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IncomeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        #region configuration
        public IncomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion
        [HttpGet("{id}")]

        public IActionResult SelectByUserID(int id, IncomeRepositery incomeRepositery)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid User ID");
            }

            List<IncomeModel> incomes = incomeRepositery.SelectByUserID(id);

            if (incomes == null || incomes.Count == 0)
            {
                return NotFound("No income records found for the given User ID.");
            }

            return Ok(incomes);
        }

        [HttpGet("{id}")]

        public IActionResult SelectByIncomeID(int id, IncomeRepositery incomeRepositery)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid User ID");
            }

            IncomeModel incomes = incomeRepositery.SelectByIncomeID(id);

            if (incomes == null)
            {
                return NotFound("No income records found for the given User ID.");
            }

            return Ok(incomes);
        }

        [HttpDelete("{id}")]


        public IActionResult Delete(int id)
        {
            string cs = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection conn = new SqlConnection(cs);

            conn.Open();

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "DeleteIncome";
            cmd.Parameters.AddWithValue("@IncomeID", id);

            int rowAffected = cmd.ExecuteNonQuery();

            conn.Close();

            if(rowAffected > 0)
            {
                return Ok("SuccessFully Deleted");
            }
            else
            {
                return BadRequest();
            }

        }


        [HttpPost]

        public IActionResult InsertIncome([FromBody] IncomeModel model, IncomeRepositery incomeRepositery)
        {
            if(model == null)
            {
                return BadRequest();
            }

            bool isInserted = incomeRepositery.InsertIncome(model);

            if(isInserted)
            {
                return Ok("Insert Successful");
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut]
        public IActionResult updateIncome([FromBody] IncomeModel income, IncomeRepositery incomeRepositery)
        {
            if (income == null)
            {
                return BadRequest();
            }
            bool isUpdated = incomeRepositery.UpdateIncome(income);
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
