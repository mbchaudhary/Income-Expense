using Income_ExpenseApiManager.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace Income_ExpenseApiManager.Data
{
    public class RegistrationRepositery
    {
        private readonly IConfiguration _configuration;

        public RegistrationRepositery(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool registration(UserModel user)
        {
            String connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO Users(UserName, Email, Password) VALUES ('"+user.UserName+ "', '"+user.Email+ "', '"+user.Password+"');", conn);
            int rowAffected = cmd.ExecuteNonQuery();
            conn.Close();

            return rowAffected > 0;
        }


        public Dictionary<string,dynamic> login(string email , string password)
        {
            String connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlDataAdapter da = new SqlDataAdapter("select * from Users where Email = '" + email + "' and Password = '" + password + "'", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            conn.Close();

            Dictionary<string, dynamic> res = new Dictionary<string, dynamic>();
            if(dt.Rows.Count > 0)
            {
                res.Add("statusCode", 200 );
                res.Add("message", "sucess" );
                res.Add("userId", dt.Rows[0]["UserID"]);
            }
            else
            {
                res.Add("statusCode",  403);
                res.Add("message", "Invalid email or password");
            }

            return res;
        }
    }
}
