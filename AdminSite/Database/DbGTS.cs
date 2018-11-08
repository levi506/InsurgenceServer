using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdminSite.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace AdminSite.Database
{
    internal static class DbGTS
    {
        public static async Task<List<GTSObject>> GetClosedGTSTrades()
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected)
            {
                conn.Close();
                return new List<GTSObject>();
            }
            var l = new List<GTSObject>();
            const string command = "SELECT id, Offer, Result, user_id, ownername, username FROM GTS WHERE Accepted=1";
            var mcom = new MySqlCommand(command, conn.Connection);
            var r = await mcom.ExecuteReaderAsync();
            while (r.Read())
            {
                l.Add(new GTSObject()
                {
                    Id = (int)r["id"],
                    Offer = JsonConvert.DeserializeObject<Models.Pokemon>(r["Offer"].ToString()),
                    Result = JsonConvert.DeserializeObject<Models.Pokemon>(r["Result"].ToString()),
                    UserId = (uint)r["user_id"],
                    Accepted = true,
                    OwnerName = r["ownername"].ToString(),
                    TraderName = r["username"].ToString()
                });
            }
            conn.Close();
            return l;
        }
        public static async Task<List<GTSObject>> GetOpenGTSTrades()
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected)
            {
                conn.Close();
                return new List<GTSObject>();
            }
            var l = new List<GTSObject>();
            const string command = "SELECT id, Offer, Request, user_id, ownername FROM GTS WHERE Accepted=0";
            var mcom = new MySqlCommand(command, conn.Connection);
            var r = await mcom.ExecuteReaderAsync();
            while (r.Read())
            {
                l.Add(new GTSObject
                {
                    Id = (int) r["id"],
                    Offer = JsonConvert.DeserializeObject<Models.Pokemon>(r["Offer"].ToString()),
                    Request = JsonConvert.DeserializeObject<GTSFilter>(r["Request"].ToString()),
                    UserId = (uint)r["user_id"],
                    Accepted = false,
                    OwnerName = r["ownername"].ToString()
                });
            }
            conn.Close();
            return l;
        }

        public static async Task<GTSObject> GetSingleGTSTrade(int i)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected)
            {
                conn.Close();
                return null;
            }
            const string command = "SELECT id, Offer, Request, user_id, Accepted, ownername, username, Result FROM GTS WHERE id = @id";
            var mcom = new MySqlCommand(command, conn.Connection);
            mcom.Parameters.AddWithValue("@id", i);
            var r = await mcom.ExecuteReaderAsync();
            GTSObject obj = null;
            while (r.Read())
            {
                Models.Pokemon result = null;
                if (!(r["Result"] is DBNull))
                    result = JsonConvert.DeserializeObject<Models.Pokemon>(r["Result"].ToString());
                obj = new GTSObject
                {
                    Id = (int)r["id"],
                    Offer = JsonConvert.DeserializeObject<Models.Pokemon>(r["Offer"].ToString()),
                    Request = JsonConvert.DeserializeObject<GTSFilter>(r["Request"].ToString()),
                    UserId = (uint)r["user_id"],
                    Accepted = (bool)r["Accepted"],
                    TraderName = r["username"].ToString(),
                    OwnerName = r["ownername"].ToString(),
                    Result = result
                };
            }
            conn.Close();
            return obj;
        }

        public static async Task DeleteGTS(int id)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected)
            {
                conn.Close();
                return;
            }
            const string command = "DELETE FROM GTS WHERE id = @id";
            var mcom = new MySqlCommand(command, conn.Connection);
            mcom.Parameters.AddWithValue("@id", id);
            await mcom.ExecuteNonQueryAsync();
            conn.Close();
        }

        public static async Task<List<GTSObject>> GetUserGTS(uint userId)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected)
            {
                conn.Close();
                return new List<GTSObject>();
            }
            var l = new List<GTSObject>();
            const string command = "SELECT id, Offer, Request, user_id, ownername, Accepted FROM GTS WHERE user_id=@id";
            var mcom = new MySqlCommand(command, conn.Connection);
            mcom.Parameters.AddWithValue("id", userId);
            var r = await mcom.ExecuteReaderAsync();
            while (r.Read())
            {
                l.Add(new GTSObject
                {
                    Id = (int) r["id"],
                    Offer = JsonConvert.DeserializeObject<Models.Pokemon>(r["Offer"].ToString()),
                    Request = JsonConvert.DeserializeObject<GTSFilter>(r["Request"].ToString()),
                    UserId = (uint)r["user_id"],
                    Accepted = (bool)r["Accepted"],
                    OwnerName = r["ownername"].ToString()
                });
            }
            conn.Close();
            return l;
        }

        public static async Task RemoveUserGTS(uint userId)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected)
            {
                conn.Close();
                return;
            }
            const string command = "DELETE FROM GTS WHERE user_id = @id";
            var mcom = new MySqlCommand(command, conn.Connection);
            mcom.Parameters.AddWithValue("@id", userId);
            await mcom.ExecuteNonQueryAsync();
            conn.Close();
        }
    }
}
