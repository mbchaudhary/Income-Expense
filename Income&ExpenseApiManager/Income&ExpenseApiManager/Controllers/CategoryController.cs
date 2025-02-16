using Income_ExpenseApiManager.Data;
using Income_ExpenseApiManager.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http;


namespace Income_ExpenseApiManager.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        #region configuration
        public CategoryController(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }
        #endregion  

        [HttpGet]
        public IActionResult GetAllCategory()
        {
            try
            {
                // Retrieve connection string from configuration
                string cs = this._configuration.GetConnectionString("ConnectionString");
                List<CategoriesModel> categories = new List<CategoriesModel>();

                // Establishing database connection
                using (SqlConnection conn = new SqlConnection(cs))
                {
                    conn.Open();

                    // Define and execute stored procedure
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SelectAllCategories";

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CategoriesModel category = new CategoriesModel
                                {
                                    CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                                    CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                                    CategoryType = reader.GetString(reader.GetOrdinal("CategoryType")),
                                    UserId = reader.IsDBNull(reader.GetOrdinal("UserId"))
                                        ? (int?)null
                                        : reader.GetInt32(reader.GetOrdinal("UserId")),
                                    CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                                        ? (DateTime?)null
                                        : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                    ModifiedAt = reader.IsDBNull(reader.GetOrdinal("ModifiedAt"))
                                        ? (DateTime?)null
                                        : reader.GetDateTime(reader.GetOrdinal("ModifiedAt"))
                                };
                                categories.Add(category);
                            }
                        }
                    }
                }

                // Check if no categories were found
                if (categories.Count == 0)
                {
                    return NotFound("No categories found.");
                }

                // Return categories if found
                return Ok(categories);
            }
            catch (SqlException sqlEx)
            {
                // Log SQL exceptions and return appropriate error response
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                // Log general exceptions and return appropriate error response
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("{id?}")]  // Make `id` optional
        public IActionResult GetExpenseDataByUserID(int? id, [FromServices] CategoryRepositery categoryRepositery)
        {
            // Validate input (only reject negative values)
            if (id.HasValue && id <= 0)
            {
                return BadRequest("Invalid User ID.");
            }

            List<CategoriesModel> model = categoryRepositery.SelectByUserID(id);

            if (model == null || model.Count == 0)
            {
                return NotFound("No categories found.");
            }

            return Ok(model);
        }




        [HttpPost]

        public IActionResult InsertCategory([FromBody] CategoriesModel model, CategoryRepositery categoryRepositery)
        {
            if (model == null)
            {
                return BadRequest();
            }

            bool isInserted = categoryRepositery.InsertCategory(model);

            if (isInserted)
            {
                return Ok("Insert Successful");
            }
            else
            {
                return BadRequest();
            }
        }


        [HttpGet("{id}")]

        public IActionResult SelectByCategoryID(int id, CategoryRepositery categoryRepositery)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid User ID");
            }

            CategoriesModel categories = categoryRepositery.SelectByCategoryID(id);

            if (categories == null)
            {
                return NotFound("No Category records found for the given User ID.");
            }

            return Ok(categories);
        }



        [HttpGet("{categoryType}")]
        public IActionResult SelectByCategoryType(string categoryType, [FromServices] CategoryRepositery categoryRepositery)
        {
            if (string.IsNullOrWhiteSpace(categoryType))
            {
                return BadRequest("Invalid Category Type.");
            }

            var categories = categoryRepositery.GetCategoryType(categoryType);

            if (categories == null || categories.Count == 0)
            {
                return NotFound("No records found for the given category type.");
            }

            return Ok(categories);
        }


        [HttpPut]
        public IActionResult CategoriesUpdate([FromBody] CategoriesModel model, [FromServices] CategoryRepositery categoryRepositery)
        {
            if (model == null || model.CategoryId <= 0)
            {
                return BadRequest(new { message = "Invalid category data." });
            }

            bool isUpdated = categoryRepositery.UpdateCategories(model);

            if (isUpdated)
            {
                return Ok(new { message = "Category updated successfully!" });
            }
            else
            {
                return NotFound(new { message = "Category not found or no changes made." });
            }
        }






        [HttpDelete("{id}")]

        public IActionResult DeletCategoty(int id)
        {
            string cs = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection conn = new SqlConnection(cs);

            conn.Open();

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "DeleteCategory";
            cmd.Parameters.AddWithValue("@CategoryId", id);
            int rowAffected = cmd.ExecuteNonQuery();

            conn.Close();

            if (rowAffected > 0)
            {
                return Ok("SuccessFully Delete");
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
