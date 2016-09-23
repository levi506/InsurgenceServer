using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace InsurgenceServer.Database
{
    public static class DbInit
    {
        public static void Connect()
        {
            var conn = new OpenConnection();
            if (conn.IsConnected())
            {
                string query = "SHOW TABLES;";
                var cmd = new MySqlCommand(query, conn.Connection);
                var reader = cmd.ExecuteReader();
                var rows = new List<string>();
                while (reader.Read())
                {
                    string row = "";
                    for (int i = 0; i < reader.FieldCount; i++)
                        row += reader.GetValue(i).ToString();
                    rows.Add(row);
                }
                reader.Close();
                foreach (var row in rows)
                {
                    Console.WriteLine("Optimizing table: " + row);
                    string optimizequery = $"OPTIMIZE TABLE {row};";
                    var optimizeCommand = new MySqlCommand(optimizequery, conn.Connection);
                    optimizeCommand.ExecuteNonQuery();
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
