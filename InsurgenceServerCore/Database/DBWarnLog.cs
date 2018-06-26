using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace InsurgenceServerCore.Database
{
    public class DBWarnLog
    {
        public static async Task LogWarning(uint userId, string reason)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
            {
                await conn.Close();
                return;
            }

            const string command = "INSERT INTO warnings (user_id, reason, time) VALUES (@param_val_1, @param_val_2, @param_val_3)";
            var m = new MySqlCommand(command, conn.Connection);
            m.Parameters.AddWithValue("@param_val_1", userId);
            m.Parameters.AddWithValue("@param_val_2", reason);
            m.Parameters.AddWithValue("@param_val_3", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            await m.ExecuteNonQueryAsync();
            await conn.Close();
        }
    }
}