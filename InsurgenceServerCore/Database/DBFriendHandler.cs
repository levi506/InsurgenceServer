using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InsurgenceServerCore.ClientHandler;
using MySql.Data.MySqlClient;

namespace InsurgenceServerCore.Database
{
    public static class DbFriendHandler
    {
        public static async Task UpdateFriends(Client client)
        {
            await UpdateFriends(client.UserId, client.Friends);
        }
        public static async Task UpdateFriends(uint userId, List<uint> friendlist)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
            {
                await conn.Close();
                return;
            }
            const string comm = "INSERT INTO friend_list (user_id,friends)" +
                                "VALUES(@uid, @friendlist)" +
                                "ON DUPLICATE KEY UPDATE" +
                                "friends = VALUES(friends)";
            var m = new MySqlCommand(comm, conn.Connection);
            m.Parameters.AddWithValue("@uid", userId);
            m.Parameters.AddWithValue("@friendlist", string.Join(",", friendlist.ToArray()));
#pragma warning disable 4014
            m.ExecuteNonQueryAsync();
#pragma warning restore 4014
            await conn.Close();
        }
        public static async Task<List<uint>> GetFriends(Client client)
        {
            return await GetFriends(client.UserId);
        }
        public static async Task<List<uint>> GetFriends(uint userId)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
            {
                await conn.Close();
                return new List<uint>();
            }
            const string comm = "SELECT friends FROM friend_list WHERE user_id = @id";
            var m = new MySqlCommand(comm, conn.Connection);
            m.Parameters.AddWithValue("@id", userId);
            var l = new List<uint>();
            var result = await m.ExecuteReaderAsync();
            if (!result.HasRows)
            {
                l = new List<uint>();
            }
            if (await result.ReadAsync())
            {
                var s = result["friends"].ToString();
                l = s.Split(',').Select(uint.Parse).ToList();
            }
            await conn.Close();
            return l;
        }
    }
}
