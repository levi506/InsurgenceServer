using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace InsurgenceServer.Database
{
    public static class DbTradelog
    {
        public static async Task LogTrade(string u1, string u2, string p1, string p2)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
            {
                await conn.Close();
                return;
            }

            const string command = "INSERT INTO newtradelog (user1, user2, pokemon1, pokemon2, time) VALUES (@param_val_1, @param_val_2, @param_val_3, @param_val_4, @param_val_5)";
            var m = new MySqlCommand(command, conn.Connection);
            m.Parameters.AddWithValue("@param_val_1", u1);
            m.Parameters.AddWithValue("@param_val_2", u2);
            m.Parameters.AddWithValue("@param_val_3", p1);
            m.Parameters.AddWithValue("@param_val_4", p2);
            m.Parameters.AddWithValue("@param_val_5", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            await m.ExecuteNonQueryAsync();
            await conn.Close();
        }
        public static async Task LogWonderTrade(string username, string pokemon)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
            {
                await conn.Close();
                return;
            }
            const string command = "INSERT INTO wondertradelog (username, pokemon, time) VALUES (@username, @pokemon, @time)";
            var m = new MySqlCommand(command, conn.Connection);
            m.Parameters.AddWithValue("@username", username);
            m.Parameters.AddWithValue("@pokemon", pokemon);
            m.Parameters.AddWithValue("@time", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

            await m.ExecuteNonQueryAsync();
            await conn.Close();
        }
    }
}
