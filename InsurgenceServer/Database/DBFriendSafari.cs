using MySql.Data.MySqlClient;
using System;

namespace InsurgenceServer.Database
{
    public static class DbFriendSafari
    {
        public static string GetBase(string username, Client client)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
            {
                conn.Close();
                return null;
            }
            const string command = "SELECT banned, base FROM users WHERE username = @param_val_1;";
            var m = new MySqlCommand(command, conn.Connection);
            m.Parameters.AddWithValue("@param_val_1", username);
            var result = m.ExecuteReader();
            if (!result.HasRows)
            {
                //We just send the login message so we don't need a special client handler just for this. User doesn't notice anyway
                client.SendMessage("<LOG result=0>");
                conn.Close();
                return null;
            }
            while (result.Read())
            {
                if ((bool)result["banned"])
                {
                    client.SendMessage($"<TRA user={username} result=1>");
                    conn.Close();
                    return null;
                }
                if (result["base"] is DBNull)
                {
                    client.SendMessage($"<VBASE user={username} result=1 base=nil>");
                    conn.Close();
                    return null;
                }
                var Base = result["base"].ToString();
                conn.Close();
                return Base;
            }
            conn.Close();
            return null;
        }
        public static void UploadBase(uint userId, string Base)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected())
            {
                const string command = "UPDATE users SET base = @param_val_1 WHERE user_id = @param_val_2;";
                var m = new MySqlCommand(command, conn.Connection);
                m.Parameters.AddWithValue("@param_val_1", Base);
                m.Parameters.AddWithValue("@param_val_2", userId);
                m.ExecuteNonQuery();
            }
            conn.Close();
        }
    }
}
