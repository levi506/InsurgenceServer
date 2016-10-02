using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminSiteNew.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace AdminSiteNew.DatabaseSpace
{
    public static class DbGTS
    {
        public static List<GTSObject> GetOpenGTSTrades()
        {
            var conn = new OpenConnection();
            if (!conn.isConnected())
            {
                conn.Close();
                return new List<GTSObject>();
            }
            var l = new List<GTSObject>();
            const string command = "SELECT id, Offer, Request, user_id, ownername FROM GTS WHERE Accepted=0";
            var mcom = new MySqlCommand(command, conn.Connection);
            var r = mcom.ExecuteReader();
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

        public static GTSObject GetSingleGTSTrade(int i)
        {
            var conn = new OpenConnection();
            if (!conn.isConnected())
            {
                conn.Close();
                return null;
            }
            const string command = "SELECT id, Offer, Request, user_id, Accepted, ownername, username Result FROM GTS WHERE id = @id";
            var mcom = new MySqlCommand(command, conn.Connection);
            mcom.Parameters.AddWithValue("@id", i);
            var r = mcom.ExecuteReader();
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

    }
}
