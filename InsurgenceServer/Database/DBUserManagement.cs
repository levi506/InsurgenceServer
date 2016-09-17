using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsurgenceServer.Database
{
    public static class DBUserManagement
    {
        public static void Ban(uint user_id, List<string> ips)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var banuser = "UPDATE users SET banned=1 WHERE user_id = @user";
                MySqlCommand m = new MySqlCommand(banuser, conn.Connection);
                m.Parameters.AddWithValue("user", user_id);
                m.ExecuteNonQuery();

                var banips = "UPDATE ips SET ipban=1 WHERE user_id = @user";
                MySqlCommand n = new MySqlCommand(banips, conn.Connection);
                n.Parameters.AddWithValue("user", user_id);
                n.ExecuteNonQuery();

                conn.Close();
            }
        }
        public static void Ban(uint user_id)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var banuser = "UPDATE users SET banned=1 WHERE user_id=@user";
                MySqlCommand m = new MySqlCommand(banuser, conn.Connection);
                m.Parameters.AddWithValue("user", user_id);
                m.ExecuteNonQuery();

                conn.Close();
            }
        }
        public static uint GetUserID(string username)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
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
