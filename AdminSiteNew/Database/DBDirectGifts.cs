using System;
using System.Collections.Generic;
using AdminSiteNew.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdminSiteNew.Database
{
    public static class DBDirectGifts
    {
        public static List<DirectGiftBase> GetGifts(string username)
        {
            var conn = new OpenConnection();
            if (!conn.isConnected())
            {
                conn.Close();
                return new List<DirectGiftBase>();
            }
            var l = new List<DirectGiftBase>();
            if (username == null)
            {
                return l;
            }

            const string command = "SELECT directgift.user_id, directgift.gifts, users.username " +
                                   "FROM directgift JOIN users " +
                                   "ON directgift.user_id = users.user_id " +
                                   "AND users.username = @username";
            var mcom = new MySqlCommand(command, conn.Connection);
            mcom.Parameters.AddWithValue("@username", username);
            var r = mcom.ExecuteReader();
            object o = null;
            while (r.Read())
            {
                o = r["gifts"];
            }

            if (o == null)
            {
                conn.Close();
                return new List<DirectGiftBase>();
            }
            var s = o.ToString();
            var json = JArray.Parse(s);
            foreach (var thing in json)
            {
                if ((DirectGiftType) Enum.Parse(typeof(DirectGiftType), thing.SelectToken("Type").ToString()) ==
                    DirectGiftType.Pokemon)
                {
                    l.Add(thing.ToObject<PokemonDirectGift>());
                }
                else
                {
                    l.Add(thing.ToObject<ItemDirectGift>());
                }
            }
            conn.Close();
            return l;
        }

        public static void SetDirectGifts(string username, List<DirectGiftBase> gifts)
        {
            var conn = new OpenConnection();
            if (!conn.isConnected())
            {
                conn.Close();
                return;
            }
            const string command = "INSERT INTO directgift (user_id, gifts) " +
                                   "VALUES ((SELECT user_id FROM users WHERE username= @username), " +
                                   " @gifts) " +
                                   "ON DUPLICATE KEY UPDATE gifts = @gifts";
            var json = JsonConvert.SerializeObject(gifts);
            var mcom = new MySqlCommand(command, conn.Connection);
            mcom.Parameters.AddWithValue("@username", username);
            mcom.Parameters.AddWithValue("@gifts", json);

            mcom.ExecuteNonQuery();
            conn.Close();
        }
    }
}
