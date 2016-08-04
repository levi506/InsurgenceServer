using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsurgenceServer.Database
{
    public static class DBInit
    {
        public static void Connect()
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                string query = "SHOW TABLES;";
                var cmd = new MySqlCommand(query, conn.Connection);
                var reader = cmd.ExecuteReader();
                var Rows = new List<string>();
                while (reader.Read())
                {
                    string row = "";
                    for (int i = 0; i < reader.FieldCount; i++)
                        row += reader.GetValue(i).ToString();
                    Rows.Add(row);
                }
                reader.Close();
                foreach (var row in Rows)
                {
                    Console.WriteLine("Optimizing table: " + row);
                    string optimizequery = string.Format("OPTIMIZE TABLE {0};", row);
                    var optimizeCommand = new MySqlCommand(optimizequery, conn.Connection);
                    var r = optimizeCommand.ExecuteNonQuery();
                }
                conn.Close();
                Console.WriteLine("Optimizing complete!");
            }
            else
            {
                conn.Close();
            }
        }
    }
}
