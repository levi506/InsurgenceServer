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
            const string command = "SELECT id, gift FROM direct_gift WHERE user_id = @id;";
            var m = new MySqlCommand(command, conn.Connection);
            m.Parameters.AddWithValue("@id", client.UserId);
            var result = m.ExecuteReader();
            string s = null;
            var id = -1;
            while (result.Read())
            {
                s = (string)result["gift"];
                id = (int)result["id"];
            }
            if (id != -1)
            {
                const string delcomm = "DELETE FROM direct_gift WHERE id = @id";
                var n = new MySqlCommand(delcomm, conn.Connection);
                n.Parameters.AddWithValue("@id", id);
                n.ExecuteNonQuery();
            }

            conn.Close();
            return s;
        }
    }
}
