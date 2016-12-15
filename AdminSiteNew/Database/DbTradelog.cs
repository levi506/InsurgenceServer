using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminSiteNew.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace AdminSiteNew.Database
{
    internal static class DbTradelog
    {
        public static List<Trade> GetUserTradeLog(string username)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var com = "SELECT * FROM tradelog WHERE @parameter IN (user1, user2) ORDER BY i DESC LIMIT 0, 100";
                MySqlCommand m = new MySqlCommand(com, conn.Connection);
                m.Parameters.AddWithValue("parameter", username);
                var l = new List<Trade>();
                var r = m.ExecuteReader();
                while (r.Read())
                {
                    var t = new Trade();
                    t.Id = (short)r["i"];
                    var u1 = (string)r["user1"];
                    var u2 = (string)r["user2"];
                    if (u1 == username)
                    {
                        t.User1 = (string)r["user1"];
                        t.User2 = (string)r["user2"];
                        t.Pokemon1 = JsonConvert.DeserializeObject<Pokemon>((string)r["pokemon1"]);
                        t.Pokemon2 = JsonConvert.DeserializeObject<Pokemon>((string)r["pokemon2"]);
                    }
                    else
                    {
                        t.User2 = (string)r["user1"];
                        t.User1 = (string)r["user2"];
                        t.Pokemon2 = JsonConvert.DeserializeObject<Pokemon>((string)r["pokemon1"]);
                        t.Pokemon1 = JsonConvert.DeserializeObject<Pokemon>((string)r["pokemon2"]);
                    }
                    l.Add(t);
                }
                conn.Close();
                return l;
            }
            conn.Close();
            return new List<Trade>();
        }
        public static List<Trade> GetTradeLog(uint startIndex)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var c = "SELECT * FROM tradelog ORDER BY i DESC LIMIT @val, 100";
                var m = new MySqlCommand(c, conn.Connection);
                m.Parameters.AddWithValue("val", startIndex);
                var l = new List<Trade>();
                var r = m.ExecuteReader();
                while (r.Read())
                {
                    var t = new Trade();
                    t.Id = (short)r["i"];
                    t.Date = (DateTime)r["time"];
                    t.User1 = (string)r["user1"];
                    t.User2 = (string)r["user2"];
                    t.Pokemon1 = JsonConvert.DeserializeObject<Pokemon>((string)r["pokemon1"]);
                    t.Pokemon2 = JsonConvert.DeserializeObject<Pokemon>((string)r["pokemon2"]);
                    l.Add(t);
                }
                conn.Close();
                return l;
            }
            conn.Close();
            return new List<Trade>();
        }

        public static List<WonderTrade> GetWonderTradeLog(uint startIndex)
        {
            var conn = new OpenConnection();
            if (!conn.isConnected())
            {
                conn.Close();
                return new List<WonderTrade>();
            }
            const string command = "SELECT * FROM wondertradelog ORDER BY id DESC LIMIT @val, 100";
            var m = new MySqlCommand(command, conn.Connection);
            m.Parameters.AddWithValue("val", startIndex);
            var l = new List<WonderTrade>();
            var r = m.ExecuteReader();
            while (r.Read())
            {
                var t = new WonderTrade();
                t.Date = (DateTime)r["time"];
                t.User = (string)r["username"];
                t.Pokemon = JsonConvert.DeserializeObject<Pokemon>((string)r["pokemon"]);
                l.Add(t);
            }
            conn.Close();
            return l;
        }

        public static Trade GetTrade(uint i)
        {
            var conn = new OpenConnection();
            if (!conn.isConnected())
            {
                conn.Close();
                return new Trade();
            }
            var c = "SELECT * FROM tradelog WHERE i = @index";
            var m = new MySqlCommand(c, conn.Connection);
            m.Parameters.AddWithValue("index", i);
            var t = new Trade();

            var r = m.ExecuteReader();
            while (r.Read())
            {
                t.Id = (short)r["i"];
                var u1 = (string)r["user1"];
                var u2 = (string)r["user2"];
                t.User1 = (string)r["user1"];
                t.User2 = (string)r["user2"];
                t.Pokemon1 = JsonConvert.DeserializeObject<Pokemon>((string)r["pokemon1"]);
                t.Pokemon2 = JsonConvert.DeserializeObject<Pokemon>((string)r["pokemon2"]);
                t.Date = (DateTime) r["time"];
            }

            return t;
        }
    }
}
