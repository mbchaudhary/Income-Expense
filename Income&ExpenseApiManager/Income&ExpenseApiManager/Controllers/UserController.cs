using Income_ExpenseApiManager.Data;
using Income_ExpenseApiManager.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Income_ExpenseApiManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserRepositery _userRepositery;

        #region configuration
        public UserController(IConfiguration configuration, UserRepositery userRepositery)
        {
            _configuration = configuration;
            _userRepositery = userRepositery;
        }
        #endregion

        [HttpGet("{id}")]

        public IActionResult selectByPKUser(int id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            UserModel userModel = _userRepositery.SelectByID(id);

            return StatusCode(200, userModel);
        }

        [HttpPut]
        public IActionResult updateCity([FromBody] UserModel user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            bool isUpdated = _userRepositery.UpdateUser(user);
            if (isUpdated)
            {
                return Ok("Successfully Updated");
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult insertUser([FromBody] UserModel user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            bool isInserted = _userRepositery.InsertUser(user);
            if (isInserted)
            {
                return Ok("Successfully inserted");
            }
            else
            {
                return BadRequest();
            }
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "DeleteUser";
            cmd.Parameters.AddWithValue("@UserID", id);

            int rowAffected = cmd.ExecuteNonQuery();
            connection.Close();

            if (rowAffected > 0)
            {
                return Ok("Delete Successfully");
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
