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
                    OwnerName = r["ownername"].ToString()
                });
            }
            return l;
        }

    }
}
