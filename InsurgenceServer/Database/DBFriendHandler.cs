using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsurgenceServer.Database
{
    public static class DBFriendHandler
    {
        public static void UpdateFriends(Client client)
        {
            UpdateFriends(client.User_Id, client.Friends);
        }
        public static void UpdateFriends(uint user_id, List<uint> friendlist)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var comm = "INSERT INTO friend_list (user_id,friends)" +
                    "VALUES(@uid, @friendlist)" +
                    "ON DUPLICATE KEY UPDATE" +
                    "friends = VALUES(friends)";
                MySqlCommand m = new MySqlCommand(comm, conn.Connection);
                m.Parameters.AddWithValue("@uid", user_id);
                m.Parameters.AddWithValue("@friendlist", string.Join(",", friendlist.ToArray()));
                m.ExecuteNonQuery();
                conn.Close();
            }
        }
        public static List<uint> GetFriends(Client client)
        {
            return GetFriends(client.User_Id);
        }
        public static List<uint> GetFriends(uint user_id)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var comm = "SELECT friends FROM friend_list WHERE user_id = @id";
                MySqlCommand m = new MySqlCommand(comm, conn.Connection);
                m.Parameters.AddWithValue("@id", user_id);
                var l = new List<uint>();
                var result = m.ExecuteReader();
                if (!result.HasRows)
                {
                    l = new List<uint>();
                }
                if (result.Read())
                {
                    var s = result["friends"].ToString();
                    l = s.Split(',').Select(UInt32.Parse).ToList();
                }
                conn.Close();
                return l;
            }
            return new List<uint>();
        }
    }
}
