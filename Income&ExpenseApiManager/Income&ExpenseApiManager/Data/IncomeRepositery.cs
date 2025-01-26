using Microsoft.Data.SqlClient;
using Income_ExpenseApiManager.Model;
using System.Data;

namespace Income_ExpenseApiManager.Data
{
    public class IncomeRepositery
    {
        private readonly IConfiguration _configuration;

        public IncomeRepositery(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<IncomeModel> SelectByUserID(int id)
        {
            string cs = this._configuration.GetConnectionString("ConnectionString");

            List<IncomeModel> incomeList = new List<IncomeModel>();

            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SelectAllIncomes";
                    cmd.Parameters.AddWithValue("@UserID", SqlDbType.Int).Value = id;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            incomeList.Add(new IncomeModel
                            {
                                IncomeID = Convert.ToInt32(reader["IncomeID"]),
                                UserID = Convert.ToInt32(reader["UserID"]),
                                IncomeAmount = Convert.ToDecimal(reader["IncomeAmount"]),
                                //IncomeSource = Convert.ToInt32(reader["IncomeSource"]),
                                IncomeSource = reader["CategoryName"].ToString(),
                                IncomeDate = Convert.ToDateTime(reader["IncomeDate"]),
                                Notes = reader["Notes"].ToString() ?? string.Empty,

                                
                            });
                        }
                    }
                }
            }

            return incomeList;
        }


        public IncomeModel SelectByIncomeID(int id)
        {
            string cs = this._configuration.GetConnectionString("ConnectionString");

            IncomeModel incomeList = new IncomeModel();

            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetIncomeById";
                    cmd.Parameters.AddWithValue("@IncomeID", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            incomeList =  new IncomeModel
                            {
                                IncomeID = Convert.ToInt32(reader["IncomeID"]),
                                UserID = Convert.ToInt32(reader["UserID"]),
                                IncomeAmount = Convert.ToDecimal(reader["IncomeAmount"]),
                                //IncomeSource = Convert.ToInt32(reader["IncomeSource"]),
                                IncomeSource = reader["IncomeSource"].ToString(),
                                IncomeDate = Convert.ToDateTime(reader["IncomeDate"]),
                                Notes = reader["Notes"].ToString(),

                            };
                        }
                    }
                }
            }

            return incomeList;
        }


        public bool InsertIncome(IncomeModel model)
        {
            string cs = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection Conn = new SqlConnection(cs);
            Conn.Open();

            SqlCommand cmd = Conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InsertIncome";
            cmd.Parameters.AddWithValue("@UserID", model.UserID);
            cmd.Parameters.AddWithValue("@IncomeAmount", model.IncomeAmount);
            cmd.Parameters.AddWithValue("@IncomeSource", model.IncomeSource);
            cmd.Parameters.AddWithValue("@IncomeDate", model.IncomeDate);
            cmd.Parameters.AddWithValue("@Notes", model.Notes);

            int rowAffected = cmd.ExecuteNonQuery();

            Conn.Close();

            return rowAffected > 0;
        }

        public bool UpdateIncome(IncomeModel income)
        {
            string ConnectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection conn = new SqlConnection(ConnectionString);
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "UpdateIncome";
            cmd.Parameters.AddWithValue("@IncomeID", income.IncomeID);

            cmd.Parameters.AddWithValue("@IncomeAmount", income.IncomeAmount);
            cmd.Parameters.AddWithValue("@IncomeSource", income.IncomeSource);
            cmd.Parameters.AddWithValue("@IncomeDate", income.IncomeDate);
            cmd.Parameters.AddWithValue("@Notes", income.Notes);

            int rowsAffected = cmd.ExecuteNonQuery();
            conn.Close();

            return rowsAffected > 0;
        }


    }
}
