using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminSiteNew.Models;
using MySql.Data.MySqlClient;

namespace AdminSiteNew.Database
{
    public static class DbMetrics
    {
        public static async Task<List<Metrics>> GetMetrics()
        {
            var l = new List<Metrics>();
            var conn = new OpenConnection();
            if (!conn.isConnected())
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

            return l;
        }
    }
}
