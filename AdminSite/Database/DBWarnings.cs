using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdminSite.Models;
using MySql.Data.MySqlClient;

namespace AdminSite.Database
{
    public class DBWarnings
    {
        public static async Task<List<WarningsModel>> GetMetrics()
        {
            var l = new List<WarningsModel>();
            var conn = new OpenConnection();
            if (!conn.IsConnected)
            {
                conn.Close();
                return l;
            }

            const string command =
                "SELECT warnings.*, users.username FROM warnings INNER JOIN users ON warnings.user_id = users.user_id ORDER BY warnings.id DESC";
            var mcom = new MySqlCommand(command, conn.Connection);
            var r = await mcom.ExecuteReaderAsync();
            while (r.Read())
            {
                var m = new WarningsModel
                {
                    Id = (uint)r["id"],
                    Username = (string)r["username"],
                    UserId = (uint)r["user_id"],
                    Reason = (string)r["reason"],
                    Time = (DateTime) r["time"]
                };
                l.Add(m);
            }
            conn.Close();
            return l;
        }

    }
}