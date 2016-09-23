using MySql.Data.MySqlClient;
using System;

namespace InsurgenceServer.Database
{
    public static class DbTradelog
    {
        public static void LogTrade(string u1, string u2, string p1, string p2)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
            {
                conn.Close();
                return;
            }

            const string command = "INSERT INTO tradelog (user1, user2, pokemon1, pokemon2, time) VALUES (@param_val_1, @param_val_2, @param_val_3, @param_val_4, @param_val_5)";
            var m = new MySqlCommand(command, conn.Connection);
            m.Parameters.AddWithValue("@param_val_1", u1);
            m.Parameters.AddWithValue("@param_val_2", u2);
            m.Parameters.AddWithValue("@param_val_3", p1);
            m.Parameters.AddWithValue("@param_val_4", p2);
            m.Parameters.AddWithValue("@param_val_5", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            m.ExecuteNonQuery();
            conn.Close();
        }
    }
}
