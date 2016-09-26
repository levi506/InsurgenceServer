using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace InsurgenceServer.Database
{
    public static class DbMetrics
    {
        public static void MetricCountOne(int key)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
            {
                conn.Close();
                return;
            }
            const string command = "INSERT INTO CounterMetrics (id, value) VALUES (@keyname, 1) ON DUPLICATE KEY UPDATE value = value + 1";
            var mcom = new MySqlCommand(command, conn.Connection);
            mcom.Parameters.AddWithValue("@keyname", key);
            mcom.ExecuteScalar();
        }

        public static int GetMetricValue(int key)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
            {
                conn.Close();
                return 0;
            }
            const string command = "SELECT value FROM CounterMetrics WHERE id=@key LIMIT 1";
            var mcom = new MySqlCommand(command, conn.Connection);
            mcom.Parameters.AddWithValue("@key", key);
            var val = mcom.ExecuteScalar();
            if (val is DBNull)
            {
                return 0;
            }
            return (int) val;
        }
    }
}
