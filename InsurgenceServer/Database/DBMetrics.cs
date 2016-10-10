using System;
using System.Collections.Generic;
using System.Data;
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
            conn.Close();
            if (val == null || val is DBNull)
            {
                return 0;
            }           
            return (int) val;
        }

        public static void MetricCountMultiple(List<int> ls)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
            {
                conn.Close();
                return;
            }
            var dic = new Dictionary<int, int>();
            foreach (var i in ls)
            {
                if (dic.ContainsKey(i))
                    dic[i]++;
                else
                    dic.Add(i, 1);
            }
            var sCommand = new StringBuilder("INSERT INTO CounterMetrics (id, value) VALUES ");

            var rows = dic.Select(kp => $"('{kp.Key}','{kp.Value}')").ToList();
            sCommand.Append(string.Join(",", rows));
            sCommand.Append(";");
            var m = new MySqlCommand(sCommand.ToString(), conn.Connection) {CommandType = CommandType.Text};
            m.ExecuteNonQuery();
        }
    }
}
