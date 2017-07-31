using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace InsurgenceServer.Database
{
    public static class DbUserChecks
    {
        public static async Task<bool> UserExists(string username)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
            {
                await conn.Close();
                return false;

            }
            const string command = "SELECT COUNT(*) FROM users WHERE username = @param_val_1;";
            var m = new MySqlCommand(command, conn.Connection);
            m.Parameters.AddWithValue("@param_val_1", username);
            var result = int.Parse((await m.ExecuteScalarAsync()).ToString());

            await conn.Close();
            return result > 0;
        }
        public static async Task<bool> UserBanned(string username)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
            {
                await conn.Close();
                return false;
            }
            uint userid = 0;
            const string command = "SELECT users.user_id, banned FROM users WHERE username = @param_val_1;";
            var m = new MySqlCommand(command, conn.Connection);
            m.Parameters.AddWithValue("@param_val_1", username);
            var result = await m.ExecuteReaderAsync();
            while (result.Read())
            {
                if ((bool)result["banned"])
                {
                    await conn.Close();
                    return true;
                }
                userid = (uint)result["user_id"];
            }
            result.Close();
            const string ipcommand = "SELECT ipban FROM ips WHERE user_id = @param_val_1;";
            var n = new MySqlCommand(ipcommand, conn.Connection);
            n.Parameters.AddWithValue("@param_val_1", userid);
            var ipresult = await n.ExecuteReaderAsync();
            while (await ipresult.ReadAsync())
            {
                var o = ipresult["ipban"];
                if (o is DBNull) continue;
                if (o is sbyte)
                {
                    var ipban = (sbyte)o;
                    if (ipban == 0 || ipban == -1) continue;
                    await conn.Close();
                    return true;
                }
                else if (o is bool)
                {
                    if (!(bool) o) continue;
                    await conn.Close();
                    return true;
                }
            }
            await conn.Close();
            return false;
        }
    }
}
