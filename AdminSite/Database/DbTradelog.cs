using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdminSite.Models;
using AdminSite.Utilities;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdminSite.Database
{
    internal static class DbTradelog
    {
        public static async Task<List<Trade>> GetUserTradeLog(string username)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected)
            {
                const string com =
                    "SELECT * FROM newtradelog WHERE @parameter IN (user1, user2) ORDER BY i DESC LIMIT 0, 100";
                var m = new MySqlCommand(com, conn.Connection);
                m.Parameters.AddWithValue("parameter", username);
                var l = new List<Trade>();
                using (var r = await m.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync())
                    {
                        var t = new Trade {Id = (int) r["i"]};
                        var u1 = (string)r["user1"];
                        if (u1 == username)
                        {
                            t.User1 = (string)r["user1"];
                            t.User2 = (string)r["user2"];
                            t.Pokemon1 = Deserializer.DeserializePokemon((string)r["pokemon1"]);
                            t.Pokemon2 = Deserializer.DeserializePokemon((string)r["pokemon2"]);
                        }
                        else
                        {
                            t.User2 = (string)r["user1"];
                            t.User1 = (string)r["user2"];
                            t.Pokemon2 = Deserializer.DeserializePokemon((string)r["pokemon1"]);
                            t.Pokemon1 = Deserializer.DeserializePokemon((string)r["pokemon2"]);
                        }
                        l.Add(t);
                    }
                }
                conn.Close();
                return l;
            }
            conn.Close();
            return new List<Trade>();
        }

        public static async Task<List<WonderTrade>> GetUserWonderTradeLog(string username)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected)
            {
                const string com =
                    "SELECT * FROM wondertradelog WHERE username = @parameter ORDER BY id DESC LIMIT 0, 100";
                var m = new MySqlCommand(com, conn.Connection);
                m.Parameters.AddWithValue("parameter", username);
                var l = new List<WonderTrade>();
                using (var r = await m.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync())
                    {
                        var t = new WonderTrade
                        {
                            Id = (uint) r["id"],
                            Pokemon = Deserializer.DeserializePokemon((string) r["pokemon"]),
                            User = username,
                            Date = (DateTime) r["time"]
                        };
                        l.Add(t);
                    }
                }
                conn.Close();
                return l;
            }
            conn.Close();
            return new List<WonderTrade>();
        }


        public static async Task<List<Trade>> GetTradeLog(uint startIndex)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected)
            {
                const string c = "SELECT * FROM newtradelog ORDER BY i DESC LIMIT @val, 100";
                var m = new MySqlCommand(c, conn.Connection);
                m.Parameters.AddWithValue("val", startIndex);
                var l = new List<Trade>();
                using (var r = await m.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync())
                    {
                        var t = new Trade
                        {
                            Id = (int) r["i"],
                            Date = (DateTime) r["time"],
                            User1 = (string) r["user1"],
                            User2 = (string) r["user2"],
                            Pokemon1 = Deserializer.DeserializePokemon((string) r["pokemon1"]),
                            Pokemon2 = Deserializer.DeserializePokemon((string) r["pokemon2"]),
                        };
                        l.Add(t);
                    }
                }
                conn.Close();
                return l;
            }
            conn.Close();
            return new List<Trade>();
        }
        public static void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            Console.WriteLine(currentError);
            errorArgs.ErrorContext.Handled = true;
        }

        public static async Task<List<WonderTrade>> GetWonderTradeLog(uint startIndex)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected)
            {
                conn.Close();
                return new List<WonderTrade>();
            }
            const string command = "SELECT * FROM wondertradelog ORDER BY id DESC LIMIT @val, 100";
            var m = new MySqlCommand(command, conn.Connection);
            m.Parameters.AddWithValue("val", startIndex);
            var l = new List<WonderTrade>();
            using (var r = await m.ExecuteReaderAsync())
            {
                while (r.Read())
                {
                    var t = new WonderTrade
                    {
                        Id = (uint)r["id"],
                        Date = (DateTime) r["time"],
                        User = (string) r["username"],
                        Pokemon = Deserializer.DeserializePokemon((string) r["pokemon"])
                    };
                    l.Add(t);
                }
            }
            conn.Close();
            return l;
        }

        public static async Task<Trade> GetTrade(uint i)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected)
            {
                conn.Close();
                return new Trade();
            }
            const string c = "SELECT * FROM newtradelog WHERE i = @index";
            var m = new MySqlCommand(c, conn.Connection);
            m.Parameters.AddWithValue("index", i);
            var t = new Trade();

            using (var r = await m.ExecuteReaderAsync())
            {
                while (r.Read())
                {
                    t.Id = (int)r["i"];
                    t.User1 = (string)r["user1"];
                    t.User2 = (string)r["user2"];
                    t.Pokemon1 = Deserializer.DeserializePokemon((string)r["pokemon1"]);
                    t.Pokemon2 = Deserializer.DeserializePokemon((string)r["pokemon2"]);
                    t.Date = (DateTime) r["time"];
                }
            }
            conn.Close();
            return t;
        }

        public static async Task<WonderTrade> GetWonderTrade(uint i)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected)
            {
                conn.Close();
                return new WonderTrade();
            }
            const string c = "SELECT * FROM wondertradelog WHERE id = @index";
            var m = new MySqlCommand(c, conn.Connection);
            m.Parameters.AddWithValue("index", i);
            var t = new WonderTrade();

            using (var r = await m.ExecuteReaderAsync())
            {
                while (r.Read())
                {
                    t.Id = (uint)r["id"];
                    t.User = (string)r["username"];
                    t.Pokemon = Deserializer.DeserializePokemon((string)r["pokemon"]);
                    t.Date = (DateTime) r["time"];
                }
            }
            conn.Close();
            return t;
        }
    }
}
