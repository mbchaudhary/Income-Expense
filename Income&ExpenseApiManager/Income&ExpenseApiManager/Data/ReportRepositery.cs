using Income_ExpenseApiManager.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Income_ExpenseApiManager.Data
{
    public class ReportRepositery
    {
        private readonly IConfiguration _configuration;

        public ReportRepositery(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public List<ReportModel> IncomeAndExpenseReportByUserId(int id)
        {
            string cs = this._configuration.GetConnectionString("ConnectionString");

            List<ReportModel> model = new List<ReportModel>();

            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SelectAllExpensesByUserId";
                    cmd.Parameters.AddWithValue("@UserID", SqlDbType.Int).Value = id;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model.Add(new ReportModel
                            {
                                Amount = Convert.ToDecimal(reader["ExpenseAmount"]),
                                Category = reader["CategoryName"].ToString(),
                                Date = Convert.ToDateTime(reader["ExpenseDate"]),
                                Notes = reader["Notes"].ToString(),
                                Type = "Expense"
                            });
                        }
                    }
                }

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SelectAllIncomes";
                    cmd.Parameters.AddWithValue("@UserID", SqlDbType.Int).Value = id;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model.Add(new ReportModel
                            {
                                Amount = Convert.ToDecimal(reader["IncomeAmount"]),
                                Category = reader["CategoryName"].ToString(),
                                Date = Convert.ToDateTime(reader["IncomeDate"]),
                                Notes = reader["Notes"].ToString() ?? string.Empty,
                                Type = "Income"
                            });
                        }
                    }
                }
            }
            return model.OrderByDescending(r => r.Date).ToList();
        }
    }
}
