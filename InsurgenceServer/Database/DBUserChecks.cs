using MySql.Data.MySqlClient;
using System;

namespace InsurgenceServer.Database
{
    public static class DbUserChecks
    {
        public static bool UserExists(string username)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected())
            {
                string command = "SELECT user_id FROM users WHERE username = @param_val_1;";
                MySqlCommand m = new MySqlCommand(command, conn.Connection);
                m.Parameters.AddWithValue("@param_val_1", username);
                var result = m.ExecuteReader();
                if (!result.HasRows)
                {
                    conn.Close();
                    return false;
                }
                else
                {
                    conn.Close();
                    return true;
                }
            }
            else
            {
                conn.Close();
                return false;
            }
        }
        public static bool UserBanned(string username)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected())
            {
                uint userid = 0;
                string command = "SELECT user_id, banned FROM users WHERE username = @param_val_1;";
                MySqlCommand m = new MySqlCommand(command, conn.Connection);
                m.Parameters.AddWithValue("@param_val_1", username);
                var result = m.ExecuteReader();
                while (result.Read())
                {
                    if ((bool)result["banned"])
                    {
                        conn.Close();
                        return true;
                    }
                    userid = (uint)result["user_id"];
                }
                result.Close();
                string ipcommand = "SELECT ipban FROM ips WHERE user_id = @param_val_1;";
                MySqlCommand n = new MySqlCommand(ipcommand, conn.Connection);
                n.Parameters.AddWithValue("@param_val_1", userid);
                var ipresult = n.ExecuteReader();
                while (ipresult.Read())
                {
                    var o = ipresult["ipban"];
                    if (o.GetType() != typeof(DBNull))
                    {
                        if (o is sbyte)
                        {
                            var ipban = (sbyte)o;
                            if (ipban != 0 && ipban != -1)
                            {
                                conn.Close();
                                return true;
                            }
                        }
                        else if (o is bool)
                        {
                            if ((bool)o)
                            {
                                conn.Close();
                                return true;
                            }
                        }

                    }
                }
                conn.Close();
                return false;
            }
            else
            {
                conn.Close();
                return false;
            }
        }
    }
}
