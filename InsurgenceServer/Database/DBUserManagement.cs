using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace InsurgenceServer.Database
{
    public static class DbUserManagement
    {
        public static void Ban(uint userId, List<string> ips)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected())
            {
                var banuser = "UPDATE users SET banned=1 WHERE user_id = @user";
                MySqlCommand m = new MySqlCommand(banuser, conn.Connection);
                m.Parameters.AddWithValue("user", userId);
                m.ExecuteNonQuery();

                var banips = "UPDATE ips SET ipban=1 WHERE user_id = @user";
                MySqlCommand n = new MySqlCommand(banips, conn.Connection);
                n.Parameters.AddWithValue("user", userId);
                n.ExecuteNonQuery();

                conn.Close();
            }
        }
        public static void Ban(uint userId)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected())
            {
                var banuser = "UPDATE users SET banned=1 WHERE user_id=@user";
                MySqlCommand m = new MySqlCommand(banuser, conn.Connection);
                m.Parameters.AddWithValue("user", userId);
                m.ExecuteNonQuery();

                conn.Close();
            }
        }
        public static uint GetUserId(string username)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected())
            {
                var banuser = "SELECT user_id FROM users WHERE username = @name";
                MySqlCommand m = new MySqlCommand(banuser, conn.Connection);
                m.Parameters.AddWithValue("name", username);
                var result = m.ExecuteReader();
                uint i = 0;
                if (result.Read())
                {
                    i = uint.Parse(result["user_id"].ToString());
                }
                conn.Close();
                return i;
            }
            return 0;
        }
    }
}
