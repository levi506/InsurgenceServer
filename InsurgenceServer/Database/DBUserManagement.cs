using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InsurgenceServer.Database
{
    public static class DbUserManagement
    {
        public static async Task Ban(uint userId, List<string> ips)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
                return;
            const string banuser = "UPDATE users SET banned=1 WHERE user_id = @user";
            var m = new MySqlCommand(banuser, conn.Connection);
            m.Parameters.AddWithValue("user", userId);
            await m.ExecuteNonQueryAsync();

            const string banips = "UPDATE ips SET ipban=1 WHERE user_id = @user";
            var n = new MySqlCommand(banips, conn.Connection);
            n.Parameters.AddWithValue("user", userId);
            await n.ExecuteNonQueryAsync();

            await conn.Close();
        }
        public static async Task Ban(uint userId)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
                return;
            const string banuser = "UPDATE users SET banned=1 WHERE user_id=@user";
            var m = new MySqlCommand(banuser, conn.Connection);
            m.Parameters.AddWithValue("user", userId);
            await m.ExecuteNonQueryAsync();

            await conn.Close();
        }
        public static async Task<uint> GetUserId(string username)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
                return 0;
            const string banuser = "SELECT user_id FROM users WHERE username = @name";
            var m = new MySqlCommand(banuser, conn.Connection);
            m.Parameters.AddWithValue("name", username);
            var result = await m.ExecuteReaderAsync();
            uint i = 0;
            if (result.Read())
            {
                i = uint.Parse(result["user_id"].ToString());
            }
            await conn.Close();
            return i;
        }
    }
}
