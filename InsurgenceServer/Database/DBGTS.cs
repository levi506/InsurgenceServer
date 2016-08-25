using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsurgenceServer.Database
{
    public static class DBGTS
    {
        public static int GetNumberOfTrades(uint userid)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var com = "SELECT COUNT(*) FROM GTS WHERE user_id=@id";
                var mcom = new MySqlCommand(com, conn.Connection);
                mcom.Parameters.AddWithValue("@id", userid);
                var i = int.Parse(mcom.ExecuteScalar().ToString());
                conn.Close();
                return i;
            }
            return 0;
        }
        public static void Add(uint userid, string offer, string request)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var com = "INSERT INTO GTS (user_id, Offer, Request) VALUES (@id, @offer, @request)";
                var mcom = new MySqlCommand(com, conn.Connection);
                mcom.Parameters.AddWithValue("@id", userid);
                mcom.Parameters.AddWithValue("@offer", offer);
                mcom.Parameters.AddWithValue("@request", request);
                mcom.ExecuteNonQuery();
                conn.Close();
            }
        }
        public static List<GTS.RequestGTSHolder> GetTrades(uint StartingIndex)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var com = "SELECT id, Offer, Request, Accepted FROM GTS WHERE Accepted = 0 LIMIT @index, 5";
                var mcom = new MySqlCommand(com, conn.Connection);
                mcom.Parameters.AddWithValue("@index", StartingIndex);
                var r = mcom.ExecuteReader();
                var ls = new List<GTS.RequestGTSHolder>();
                while (r.Read())
                {
                    Console.WriteLine(r["id"].GetType());
                    var h = new GTS.RequestGTSHolder
                    {
                        Index = (int)r["id"],
                        Offer = JsonConvert.DeserializeObject<GTS.GamePokemon>((string)r["Offer"]),
                        Request = JsonConvert.DeserializeObject<GTS.RequestData>((string)r["Request"]),
                        Accepted = (bool)r["Accepted"]
                    };
                    Console.WriteLine("2");
                    ls.Add(h);
                }
                conn.Close();
                return ls;
            }
            return null;
        }
        public static GTS.RequestGTSHolder GetSingleTrade(uint index)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var s = "SELECT Offer, Request FROM GTS WHERE id = @id";
                var c = new MySqlCommand(s, conn.Connection);
                c.Parameters.AddWithValue("@id", index);
                var r = c.ExecuteReader();
                GTS.RequestGTSHolder ret = null;
                while (r.Read())
                {
                    ret = new GTS.RequestGTSHolder
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
            return null;
        }
        public static bool GetAccepted(uint index)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var s = "SELECT Accepted FROM GTS WHERE id=@index";
                var c = new MySqlCommand(s, conn.Connection);
                c.Parameters.AddWithValue("@id", index);
                var r = c.ExecuteReader();
                bool ret = false;
                while (r.Read())
                {
                    if ((bool)r["Accepted"])
                        ret = true;
                }
                conn.Close();
                return ret;
            }
            return false;
        }
        public static void SetAccepted(uint index, string pokemon)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var s = "UPDATE GTS SET Accepted=1, Result=@poke";
                var c = new MySqlCommand(s, conn.Connection);
                c.Parameters.AddWithValue("@poke", pokemon);
                c.ExecuteNonQuery();
                conn.Close();
            }
        }
        public static List<GTS.RequestGTSHolder> GetUserTrades(uint user_id)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var s = "SELECT Offer, Request, Accepted FROM GTS WHERE user_id = @id";
                var c = new MySqlCommand(s, conn.Connection);
                c.Parameters.AddWithValue("@id", user_id);
                var ls = new List<GTS.RequestGTSHolder>();
                var r = c.ExecuteReader();
                while (r.Read())
                {
                    var req = new GTS.RequestGTSHolder
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
            return null;
        }
        public static bool UserOwnsTrade(uint trade_id, uint user_id)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var s = "SELECT user_id WHERE trade_id=@id";
                var c = new MySqlCommand(s, conn.Connection);
                c.Parameters.AddWithValue("@id", trade_id);
                var r = c.ExecuteReader();
                bool ret = false;
                while (r.Read())
                {
                    var user = (uint)r["user_id"];
                    if (user == user_id)
                        ret = true;
                }
                conn.Close();
                return ret;
            }
            return false;
        }
        public static void CancelTrade(uint tradeid)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var s = "DELETE FROM GTS WHERE id = @id";
                var c = new MySqlCommand(s, conn.Connection);
                c.Parameters.AddWithValue("@id", tradeid);
                c.ExecuteNonQuery();
                conn.Close();
            }
        }
        public static string CollectTrade(uint tradeid)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var s = "SELECT Result FROM GTS WHERE id=@id";
                var c = new MySqlCommand(s, conn.Connection);
                c.Parameters.AddWithValue("@id", tradeid);
                string ret = "";
                var r = c.ExecuteReader();
                while (r.Read())
                {
                    ret = (string)r["Result"];
                }
                var delete = "DELETE FROM GTS WHERE id = @id";
                var deletecom = new MySqlCommand(delete, conn.Connection);
                deletecom.Parameters.AddWithValue("@id", tradeid);
                deletecom.ExecuteNonQuery();
                conn.Close();
                return ret;
            }
            return null;
        }
        public static bool TradeIsAccepted(uint tradeid)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var s = "SELECT Accepted WHERE trade_id=@id";
                var c = new MySqlCommand(s, conn.Connection);
                c.Parameters.AddWithValue("@id", tradeid);
                var r = c.ExecuteReader();
                bool ret = false;
                while (r.Read())
                {
                    var accepted = (bool)r["Accepted"];
                    if (accepted)
                        ret = true;
                }
                conn.Close();
                return ret;
            }
            return false;
        }
    }
}
