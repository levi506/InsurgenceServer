using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace InsurgenceServer.Database
{
    public static class Dbgts
    {
        public static int GetNumberOfTrades(uint userid)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
                return 0;
            const string com = "SELECT COUNT(*) FROM GTS WHERE user_id=@id";
            var mcom = new MySqlCommand(com, conn.Connection);
            mcom.Parameters.AddWithValue("@id", userid);
            var i = int.Parse(mcom.ExecuteScalar().ToString());
            conn.Close();
            return i;
        }
        public static void Add(uint userid, string offer, string request, int level, string ownername)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
                return;
            const string com = "INSERT INTO GTS (user_id, Offer, Request, OfferLevel, ownername) VALUES (@id, @offer, @request, @level, @ownername)";
            var mcom = new MySqlCommand(com, conn.Connection);
            mcom.Parameters.AddWithValue("@id", userid);
            mcom.Parameters.AddWithValue("@offer", offer);
            mcom.Parameters.AddWithValue("@request", request);
            mcom.Parameters.AddWithValue("@level", level);
            mcom.Parameters.AddWithValue("@ownername", ownername);
            mcom.ExecuteNonQuery();
            conn.Close();
        }
        public static List<GTS.RequestGtsHolder> GetTrades(uint startingIndex, GTS.FilterHolder filter)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
                return null;
            var com = "SELECT id, Offer, Request, Accepted FROM GTS " +
                      "WHERE (Accepted = 0 " +
                      "AND OfferLevel >= @minLevel";
            if (filter.Species != 0)
            {
                com += " AND (Offer->'$.species') = @species";
            }
            const int ignoreNature = 25;
            if (filter.Nature != ignoreNature)
            {
                //com += " AND ()"
            }
            const int ignoreGender = 0;
            if (filter.Gender != ignoreGender)
            {
                //com += " AND (Offer->'$.')"
            }
            com += ") LIMIT @index, 4";
            var mcom = new MySqlCommand(com, conn.Connection);
            mcom.Parameters.AddWithValue("@index", startingIndex);
            mcom.Parameters.AddWithValue("@minLevel", filter.MinLevel);
            if (filter.Species != 0)
            {
                mcom.Parameters.AddWithValue("@species", filter.Species);
            }
            var r = mcom.ExecuteReader();
            var ls = new List<GTS.RequestGtsHolder>();
            while (r.Read())
            {
                var h = new GTS.RequestGtsHolder
                {
                    Index = (int)r["id"],
                    Offer = JsonConvert.DeserializeObject<GTS.GamePokemon>((string)r["Offer"]),
                    Request = JsonConvert.DeserializeObject<GTS.RequestData>((string)r["Request"]),
                    Accepted = (bool)r["Accepted"]
                };
                ls.Add(h);
            }
            conn.Close();
            return ls;
        }
        public static GTS.RequestGtsHolder GetSingleTrade(uint index)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
                return null;
            const string s = "SELECT id, Offer, Request, Accepted FROM GTS WHERE id = @id";
            var c = new MySqlCommand(s, conn.Connection);
            c.Parameters.AddWithValue("@id", index);
            var r = c.ExecuteReader();
            GTS.RequestGtsHolder ret = null;
            while (r.Read())
            {
                ret = new GTS.RequestGtsHolder
                {
                    Index = (int)r["id"],
                    Offer = JsonConvert.DeserializeObject<GTS.GamePokemon>((string)r["Offer"]),
                    Request = JsonConvert.DeserializeObject<GTS.RequestData>((string)r["Request"]),
                    Accepted = (bool)r["Accepted"]
                };
            }
            conn.Close();
            return ret;
        }
        public static bool GetAccepted(uint index)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
                return false;
            const string s = "SELECT Accepted FROM GTS WHERE id=@index";
            var c = new MySqlCommand(s, conn.Connection);
            c.Parameters.AddWithValue("@id", index);
            var r = c.ExecuteReader();
            var ret = false;
            while (r.Read())
            {
                if ((bool)r["Accepted"])
                    ret = true;
            }
            conn.Close();
            return ret;
        }
        public static void SetAccepted(uint index, string pokemon)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
                return;
            const string s = "UPDATE GTS SET Accepted=1, Result=@poke";
            var c = new MySqlCommand(s, conn.Connection);
            c.Parameters.AddWithValue("@poke", pokemon);
            c.ExecuteNonQuery();
            conn.Close();
        }
        public static List<GTS.RequestGtsHolder> GetUserTrades(uint userId)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
                return null;
            const string s = "SELECT id, Offer, Request, Accepted FROM GTS WHERE user_id = @id";
            var c = new MySqlCommand(s, conn.Connection);
            c.Parameters.AddWithValue("@id", userId);
            var ls = new List<GTS.RequestGtsHolder>();
            var r = c.ExecuteReader();
            while (r.Read())
            {
                var req = new GTS.RequestGtsHolder
                {
                    Index = (int)r["id"],
                    Offer = JsonConvert.DeserializeObject<GTS.GamePokemon>((string)r["Offer"]),
                    Request = JsonConvert.DeserializeObject<GTS.RequestData>((string)r["Request"]),
                    Accepted = (bool)r["Accepted"]
                };
                ls.Add(req);
            }
            conn.Close();
            return ls;
        }
        public static bool UserOwnsTrade(uint tradeId, uint userId)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
                return false;
            const string s = "SELECT user_id FROM gts WHERE id=@id";
            var c = new MySqlCommand(s, conn.Connection);
            c.Parameters.AddWithValue("@id", tradeId);
            var r = c.ExecuteReader();
            var ret = false;
            while (r.Read())
            {
                var user = (int)r["user_id"];
                if (user == userId)
                    ret = true;
            }
            conn.Close();
            return ret;
        }
        public static void CancelTrade(uint tradeid)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
                return;
            const string s = "DELETE FROM GTS WHERE id = @id";
            var c = new MySqlCommand(s, conn.Connection);
            c.Parameters.AddWithValue("@id", tradeid);
            c.ExecuteNonQuery();
            conn.Close();
        }
        public static string CollectTrade(uint tradeid)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
                return null;
            const string s = "SELECT Result FROM GTS WHERE id=@id";
            var c = new MySqlCommand(s, conn.Connection);
            c.Parameters.AddWithValue("@id", tradeid);
            var ret = "";
            var r = c.ExecuteReader();
            while (r.Read())
            {
                ret = (string)r["Result"];
            }
            const string delete = "DELETE FROM GTS WHERE id = @id";
            var deletecom = new MySqlCommand(delete, conn.Connection);
            deletecom.Parameters.AddWithValue("@id", tradeid);
            deletecom.ExecuteNonQuery();
            conn.Close();
            return ret;
        }
        public static bool TradeIsAccepted(uint tradeid)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
                return false;
            const string s = "SELECT Accepted WHERE trade_id=@id";
            var c = new MySqlCommand(s, conn.Connection);
            c.Parameters.AddWithValue("@id", tradeid);
            var r = c.ExecuteReader();
            var ret = false;
            while (r.Read())
            {
                var accepted = (bool)r["Accepted"];
                if (accepted)
                    ret = true;
            }
            conn.Close();
            return ret;
        }
    }
}
