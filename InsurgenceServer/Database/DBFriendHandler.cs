using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InsurgenceServer.Database
{
    public static class DbFriendHandler
    {
        public static void UpdateFriends(Client client)
        {
            UpdateFriends(client.UserId, client.Friends);
        }
        public static void UpdateFriends(uint userId, List<uint> friendlist)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
            {
                conn.Close();
                return;
            }
            const string comm = "INSERT INTO friend_list (user_id,friends)" +
                                "VALUES(@uid, @friendlist)" +
                                "ON DUPLICATE KEY UPDATE" +
                                "friends = VALUES(friends)";
            var m = new MySqlCommand(comm, conn.Connection);
            m.Parameters.AddWithValue("@uid", userId);
            m.Parameters.AddWithValue("@friendlist", string.Join(",", friendlist.ToArray()));
            m.ExecuteNonQuery();
            conn.Close();
        }
        public static List<uint> GetFriends(Client client)
        {
            return GetFriends(client.UserId);
        }
        public static List<uint> GetFriends(uint userId)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
            {
                conn.Close();
                return new List<uint>();
            }
            const string comm = "SELECT friends FROM friend_list WHERE user_id = @id";
            var m = new MySqlCommand(comm, conn.Connection);
            m.Parameters.AddWithValue("@id", userId);
            var l = new List<uint>();
            var result = m.ExecuteReader();
            if (!result.HasRows)
            {
                l = new List<uint>();
            }
            if (result.Read())
            {
                var s = result["friends"].ToString();
                l = s.Split(',').Select(uint.Parse).ToList();
            }
            conn.Close();
            return l;
        }
    }
}
