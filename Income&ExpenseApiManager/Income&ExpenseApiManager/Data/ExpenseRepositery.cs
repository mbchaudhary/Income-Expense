using Microsoft.Data.SqlClient;
using Income_ExpenseApiManager.Model;
using System.Data;

namespace Income_ExpenseApiManager.Data
{
    public class ExpenseRepositery
    {
        private readonly IConfiguration _configuration;

        public ExpenseRepositery(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<ExpenseModel> SelectByIDExpense(int id)
        {
            string cs = this._configuration.GetConnectionString("ConnectionString");

            List<ExpenseModel> model = new List<ExpenseModel>();

            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();

                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SelectAllExpensesByUserId";
                    cmd.Parameters.AddWithValue("@UserId", id);

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            model.Add(new ExpenseModel
                            {
                                ExpenseId = Convert.ToInt32(reader["ExpenseId"]),
                                UserId = Convert.ToInt32(reader["UserId"]),
                                ExpenseAmount = Convert.ToDecimal(reader["ExpenseAmount"]),
                                ExpenseCategory = reader["ExpenseCategory"].ToString(),
                                ExpenseDate = Convert.ToDateTime(reader["ExpenseDate"]),
                                Notes = reader["Notes"].ToString()
                            });
                        }
                    }
                }
            }
            return model;
        }


        public bool InsertExpense(ExpenseModel model)
        {
            string cs = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection Conn = new SqlConnection(cs);
            Conn.Open();

            SqlCommand cmd = Conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InsertExpense";
            cmd.Parameters.AddWithValue("@UserId", model.UserId);
            cmd.Parameters.AddWithValue("@ExpenseAmount", model.ExpenseAmount);
            cmd.Parameters.AddWithValue("@ExpenseCategory", model.ExpenseCategory);
            cmd.Parameters.AddWithValue("@ExpenseDate", model.ExpenseDate);
            cmd.Parameters.AddWithValue("@Notes", model.Notes);

            int rowAffected = cmd.ExecuteNonQuery();

            Conn.Close();

            return rowAffected > 0;
        }


        public bool UpdateExpense(ExpenseModel model)
        {
            string cs = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection Conn = new SqlConnection(cs);
            Conn.Open();

            SqlCommand cmd = Conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "UpdateExpense";
            cmd.Parameters.AddWithValue("@ExpenseId", model.ExpenseId);

            cmd.Parameters.AddWithValue("@ExpenseAmount", model.ExpenseAmount);
            cmd.Parameters.AddWithValue("@ExpenseCategory", model.ExpenseCategory);
            cmd.Parameters.AddWithValue("@ExpenseDate", model.ExpenseDate);
            cmd.Parameters.AddWithValue("@Notes", model.Notes);

            int rowAffected = cmd.ExecuteNonQuery();

            Conn.Close();

            return rowAffected > 0;
        }

    }
}
