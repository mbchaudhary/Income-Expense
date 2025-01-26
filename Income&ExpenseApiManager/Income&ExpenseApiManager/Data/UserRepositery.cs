using Microsoft.Data.SqlClient;
using Income_ExpenseApiManager.Model;
using System.Data;

namespace Income_ExpenseApiManager.Data
{
    public class UserRepositery
    {
        private readonly IConfiguration _configuration;

        public UserRepositery(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public UserModel SelectByID(int id)
        {
            string cs = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SelectUser";
            cmd.Parameters.AddWithValue("@UserID", id);
            UserModel model = new UserModel();

            using(SqlDataReader reader = cmd.ExecuteReader())
            {
                if(reader.Read())
                {
                    model.UserID = Convert.ToInt32(reader["UserID"]);
                    model.UserName = reader["UserName"].ToString();
                    model.Email = reader["Email"].ToString();
                    model.Password = reader["Password"].ToString();
                }
            }

            return model;
        }


        public bool UpdateUser(UserModel user)
        {
            string ConnectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection conn = new SqlConnection(ConnectionString);
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "UpdateUser";
            cmd.Parameters.AddWithValue("@UserID", user.UserID);

            cmd.Parameters.AddWithValue("@UserName", user.UserName);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Password", user.Password);

            int rowsAffected = cmd.ExecuteNonQuery();
            conn.Close();

            return rowsAffected > 0;
        }


        public bool InsertUser(UserModel user)
        {
            string ConnectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection conn = new SqlConnection(ConnectionString);
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InsertUser";
            cmd.Parameters.AddWithValue("@UserName", user.UserName);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Password", user.Password);

            int rowsAffected = cmd.ExecuteNonQuery();
            conn.Close();

            return rowsAffected > 0;
        }


    }
}
