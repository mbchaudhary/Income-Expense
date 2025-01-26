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



    //public List<CategoriesDropDown> getCategoriesDropdown()
    //{
    //    string connectionString = this._configuration.GetConnectionString("ConnectionString");
    //    List<CategoriesDropDown> categoriesList = new List<CategoriesDropDown>();

    //    using (SqlConnection conn = new SqlConnection(connectionString))
    //    {
    //        conn.Open();
    //        using (SqlCommand cmd = conn.CreateCommand())
    //        {
    //            cmd.CommandType = CommandType.StoredProcedure;
    //            cmd.CommandText = "GET_Categories_DropDown";

    //            using (SqlDataReader reader = cmd.ExecuteReader())
    //            {
    //                while (reader.Read())
    //                {
    //                    CategoriesDropDown categories = new CategoriesDropDown();
    //                    categories.CategoryId = Convert.ToInt32(reader["CategoryId"]);
    //                    categories.CategoryName = reader["CategoryName"]?.ToString();
    //                    categories.CategoryType = reader["CategoryType"]?.ToString();
    //                    categoriesList.Add(categories);
    //                }
    //            }
    //        }
    //    }

    //    return categoriesList;
    //}

}
