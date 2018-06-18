using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdminSite.Models;
using MySql.Data.MySqlClient;

namespace AdminSite.Database
{
    public static class DbMetrics
    {
        public static async Task<List<Metrics>> GetMetrics()
        {
            var l = new List<Metrics>();
            var conn = new OpenConnection();
            if (!conn.IsConnected)
            {
                conn.Close();
                return l;
            }

            const string command = "SELECT * FROM CounterMetrics";
            var mcom = new MySqlCommand(command, conn.Connection);
            var r = await mcom.ExecuteReaderAsync();
            while (r.Read())
            {
                var m = new Metrics
                {
                    Key = (int)r["id"],
                    Value = (int)r["value"]
                };
                if (r["name"].GetType() != typeof(DBNull))
                {
                    m.Name = r["name"].ToString();
                }
                l.Add(m);
            }
            conn.Close();
            return l;
        }
    }
}
