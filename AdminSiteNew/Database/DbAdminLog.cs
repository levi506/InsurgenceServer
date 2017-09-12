using System.Collections.Generic;
using System.Threading.Tasks;
using AdminSiteNew.Models;
using MySql.Data.MySqlClient;

namespace AdminSiteNew.Database
{
    public class DbAdminLog
    {
        public enum LogType
        {
            UserBan,
            UserUnban,
            IpBan,
            IpUnban,
            AltsBan,
            AltsUnban,
            GtsRemove,
            GtsRemoveUser,
            DirectGiftPokemon,
            DirectGiftItem,
            DirectGiftDelete
        }
        public static async Task Log(LogType type, string moderator, string data)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected)
            {
                conn.Close();
                return;
            }
            const string commandString = "INSERT INTO adminlog (type, moderator, data) VALUES (@type, @moderator, @data)";
            var command = new MySqlCommand(commandString, conn.Connection);
            command.Parameters.AddWithValue("type", type);
            command.Parameters.AddWithValue("moderator", moderator);
            command.Parameters.AddWithValue("data", data);
            await command.ExecuteNonQueryAsync();
            conn.Close();
        }

        public static async Task<List<AdminModels.AdminLogModel>> GetLog()
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected)
            {
                conn.Close();
                return null;
            }
            const string commandString = "SELECT * FROM adminlog ORDER BY id DESC";
            var mcom = new MySqlCommand(commandString, conn.Connection);

            var ls = new List<AdminModels.AdminLogModel>();
            using (var r = await mcom.ExecuteReaderAsync())
            {
                while (await r.ReadAsync())
                {
                    ls.Add(new AdminModels.AdminLogModel
                    {
                        Id = (uint) r["id"],
                        Type = (LogType) ((int)r["type"]),
                        Moderator = (string) r["moderator"],
                        Data = (string) r["data"]
                    });
                }
            }
            conn.Close();
            return ls;
        }
    }
}