using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace AdminSiteNew.Database
{
    public static class DbUserNotes
    {
        public static async Task AddNote(uint user, string moderator, string note)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected)
            {
                conn.Close();
                return;
            }
            const string commandString = "INSERT INTO usernotes (user_id, moderator, note, time) VALUES " +
                                         "(@user, @moderator, @note, @time)";
            var command = new MySqlCommand(commandString, conn.Connection);
            command.Parameters.AddWithValue("user", user);
            command.Parameters.AddWithValue("moderator", moderator);
            command.Parameters.AddWithValue("note", note);
            command.Parameters.AddWithValue("time", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            await command.ExecuteNonQueryAsync();
            conn.Close();
        }
    }
}