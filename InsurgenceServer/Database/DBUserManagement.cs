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
    }
}
