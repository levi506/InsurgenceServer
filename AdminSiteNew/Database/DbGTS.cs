using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdminSiteNew.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace AdminSiteNew.Database
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
                    Offer = JsonConvert.DeserializeObject<Pokemon>(r["Offer"].ToString()),
                    Result = JsonConvert.DeserializeObject<Pokemon>(r["Result"].ToString()),
                    UserId = (int)r["user_id"],
                    Accepted = true,
                    OwnerName = r["ownername"].ToString(),
                    TraderName = r["username"].ToString()
                });
            }
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
                    Offer = JsonConvert.DeserializeObject<Pokemon>(r["Offer"].ToString()),
                    Request = JsonConvert.DeserializeObject<GTSFilter>(r["Request"].ToString()),
                    UserId = (int)r["user_id"],
                    Accepted = false,
                    OwnerName = r["ownername"].ToString()
                });
            }
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
            while (r.Read())
            {
                Pokemon result = null;
                if (!(r["Result"] is DBNull))
                    result = JsonConvert.DeserializeObject<Pokemon>(r["Result"].ToString());
                return new GTSObject
                {
                    Id = (int)r["id"],
                    Offer = JsonConvert.DeserializeObject<Pokemon>(r["Offer"].ToString()),
                    Request = JsonConvert.DeserializeObject<GTSFilter>(r["Request"].ToString()),
                    UserId = (int)r["user_id"],
                    Accepted = (bool)r["Accepted"],
                    TraderName = r["username"].ToString(),
                    OwnerName = r["ownername"].ToString(),
                    Result = result
                };
            }
            return null;
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
        }

    }
}
