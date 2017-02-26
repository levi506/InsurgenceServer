using MySql.Data.MySqlClient;

namespace InsurgenceServer.Database
{
    public static class DbMisc
    {
        public static string GetDirectGift(Client client)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
            {
                conn.Close();
                return null;
            }
            const string command = "SELECT gifts FROM directgift WHERE user_id = @id;";
            var m = new MySqlCommand(command, conn.Connection);
            m.Parameters.AddWithValue("@id", client.UserId);
            var result = m.ExecuteReader();
            string s = null;
            while (result.Read())
            {
                s = (string)result["gifts"];
            }
            result.Close();
            if (s != null)
            {
                const string delcomm = "DELETE FROM directgift WHERE user_id = @id";
                var n = new MySqlCommand(delcomm, conn.Connection);
                n.Parameters.AddWithValue("@id", client.UserId);
                n.ExecuteNonQuery();
            }

            conn.Close();
            return s;
        }
    }
}
