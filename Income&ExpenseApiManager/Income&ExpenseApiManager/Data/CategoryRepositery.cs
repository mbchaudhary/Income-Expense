using Microsoft.Data.SqlClient;
using Income_ExpenseApiManager.Model;
using System.Data;

namespace Income_ExpenseApiManager.Data
{
    public class CategoryRepositery
    {
        private readonly IConfiguration _configuration;

        public CategoryRepositery(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool InsertCategory(CategoriesModel model)
        {
            string cs = this._configuration.GetConnectionString("ConnectionString");

            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "InsertCategory";

                    cmd.Parameters.AddWithValue("@CategoryName", model.CategoryName);
                    cmd.Parameters.AddWithValue("@CategoryType", model.CategoryType);
                    cmd.Parameters.AddWithValue("@UserId", model.UserId ?? (object)DBNull.Value);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }


        public List<CategoriesModel> SelectByUserID(int? id)
        {
            string cs = this._configuration.GetConnectionString("ConnectionString");

            List<CategoriesModel> model = new List<CategoriesModel>();

            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetCategoriesByUserId";

                    // Handle NULL properly when passing parameters
                    cmd.Parameters.AddWithValue("@UserId", id ?? (object)DBNull.Value);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model.Add(new CategoriesModel
                            {
                                CategoryId = Convert.ToInt32(reader["CategoryId"]),

                                UserId = reader["UserId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["UserId"]),

                                CategoryName = reader["CategoryName"].ToString(),
                                CategoryType = reader["CategoryType"].ToString(),
                            });
                        }
                    }
                }
            }
            return model;
        }


        public CategoriesModel SelectByCategoryID(int id) // Changed return type to CategoriesModel
        {
            string cs = this._configuration.GetConnectionString("ConnectionString");

            CategoriesModel categories = null; // Initialize as null

            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetCategoryById";
                    cmd.Parameters.AddWithValue("@CategoryId", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) // Read only the first record
                        {
                            categories = new CategoriesModel
                            {
                                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                                UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("UserId")),
                                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                                CategoryType = reader.GetString(reader.GetOrdinal("CategoryType"))
                            };
                        }
                    }
                }
            }

            return categories; // Returns null if no record is found
        }



        public bool UpdateCategories(CategoriesModel categories)
        {
            // Return false if UserId is null to prevent unnecessary DB calls
            if (!categories.UserId.HasValue)
            {
                return false;
            }

            string cs = this._configuration.GetConnectionString("ConnectionString");

            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "UpdateCategoryByUserId";

                    cmd.Parameters.AddWithValue("@CategoryId", categories.CategoryId);
                    cmd.Parameters.AddWithValue("@CategoryName", categories.CategoryName);
                    cmd.Parameters.AddWithValue("@CategoryType", categories.CategoryType);
                    cmd.Parameters.AddWithValue("@UserId", categories.UserId.Value); // UserId is guaranteed to have a value

                    int rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }



        public List<CategoriesModel> GetCategoryType(string categoryType)
        {
            string cs = _configuration.GetConnectionString("ConnectionString");
            var categories = new List<CategoriesModel>();

            using (var conn = new SqlConnection(cs))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetCategoriesByType";

                    // Add parameter for the stored procedure
                    cmd.Parameters.AddWithValue("@CategoryType", categoryType);

                    // Execute the command and read the data
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Map each row to the CategoriesModel object
                            var category = new CategoriesModel
                            {
                                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                                CategoryType = reader.GetString(reader.GetOrdinal("CategoryType")),
                                UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("UserId")),
                                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                ModifiedAt = reader.IsDBNull(reader.GetOrdinal("ModifiedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ModifiedAt"))
                            };

                            categories.Add(category);
                        }
                    }
                }
            }

            return categories;
        }
    }
}
