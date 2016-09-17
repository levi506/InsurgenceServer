using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsurgenceServer.Database
{
    public static class DBMisc
    {
        public static string GetDirectGift(Client client)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                string command = "SELECT id, gift FROM direct_gift WHERE user_id = @id;";
                MySqlCommand m = new MySqlCommand(command, conn.Connection);
                m.Parameters.AddWithValue("@id", client.User_Id);
                var result = m.ExecuteReader();
                string s = null;
                int id = -1;
                while (result.Read())
                {
                    s = (string)result["gift"];
                    id = (int)result["id"];
                }
                if (id != -1)
                {
                    string delcomm = "DELETE FROM direct_gift WHERE id = @id";
                    MySqlCommand n = new MySqlCommand(delcomm, conn.Connection);
                    n.Parameters.AddWithValue("@id", id);
                    n.ExecuteNonQuery();
                }

                conn.Close();
                return s;
            }
            return null;
        }
    }
}
