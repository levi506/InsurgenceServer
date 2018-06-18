using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminSite.Models;
using MySql.Data.MySqlClient;

namespace AdminSite.Database
{
    internal static class Database
    {
        public static async Task<UserRequest> GetUser(string username)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected)
            {
                conn.Close();
                return null;
            }
            var u = new UserRequest {UserInfo = new UserInfo {Username = username}};

            const string usercommand = "SELECT user_id, banned, base FROM users WHERE username = @param_val_1;";
            var m = new MySqlCommand(usercommand, conn.Connection);
            m.Parameters.AddWithValue("@param_val_1", username);
            using (var result = await m.ExecuteReaderAsync())
            {
                if (!result.HasRows)
                {
                    conn.Close();
                    return null;
                }
                while (await result.ReadAsync())
                {
                    u.UserInfo.User_Id = (uint) result["user_id"];
                    if (result["banned"].GetType() != typeof(DBNull))
                        u.UserInfo.Banned = (bool) result["banned"];
                    else
                        u.UserInfo.Banned = false;
                    u.FriendSafariString = (string) result["base"];
                }
            }
            u.FriendSafari = new FriendSafari(u.FriendSafariString);

            const string getLoginData = "SELECT lastlogin FROM user_data WHERE user_id = @param_val_1";
            var glc = new MySqlCommand(getLoginData, conn.Connection);
            glc.Parameters.AddWithValue("@param_val_1", u.UserInfo.User_Id);
            using (var glcresult = await glc.ExecuteReaderAsync())
            {
                while (await glcresult.ReadAsync())
                {
                    if (glcresult["lastlogin"].GetType() != typeof(DBNull))
                    {
                        u.UserInfo.LastLoggedIn = (DateTime) glcresult["lastlogin"];
                    }
                }
            }

            //Find IPs
            const string ipcommand = "SELECT ip, ipban FROM ips WHERE user_id = @param_val_1;";
            var n = new MySqlCommand(ipcommand, conn.Connection);
            n.Parameters.AddWithValue("@param_val_1", u.UserInfo.User_Id);
            u.IPs = new List<IPInfo>();
            using (var ipresult = await n.ExecuteReaderAsync())
            {
                while (await ipresult.ReadAsync())
                {
                    bool ban;
                    if (ipresult["ipban"].GetType() != typeof(DBNull))
                    {
                        if (ipresult["ipban"] is sbyte)
                        {
                            ban = Convert.ToBoolean((sbyte) ipresult["ipban"]);
                        }
                        else
                        {
                            ban = (bool) ipresult["ipban"];
                        }
                    }
                    else
                    {
                        ban = false;
                    }
                    var ip = new IPInfo
                    {
                        IP = (string) ipresult["ip"],
                        Banned = ban
                    };
                    u.IPs.Add(ip);
                }
            }
            //Find Alts----------------------------------
            const string altipcommand = "SELECT user_id FROM ips WHERE FIND_IN_SET (ip, @param_val_1) != 0";
            var o = new MySqlCommand(altipcommand, conn.Connection);
            var ipl = u.IPs.Select(ip => ip.IP).ToList();
            o.Parameters.AddWithValue("param_val_1", string.Join(",", ipl));
            var l = new List<uint>();
            using (var altipresult = await o.ExecuteReaderAsync())
            {
                while (altipresult.Read())
                {
                    l.Add((uint) altipresult["user_id"]);
                }
            }
            const string altcommand =
                "SELECT username, user_id, banned FROM users WHERE FIND_IN_SET (user_id, @param_val_1) != 0";
            var p = new MySqlCommand(altcommand, conn.Connection);
            p.Parameters.AddWithValue("param_val_1", string.Join(",", l));
            u.Alts = new List<UserInfo>();
            using (var altresult = await p.ExecuteReaderAsync())
            {
                while (altresult.Read())
                {
                    var uinfo = new UserInfo
                    {
                        User_Id = (uint) altresult["user_id"],
                        Username = (string) altresult["username"],
                        Banned = (bool) altresult["banned"]
                    };
                    u.Alts.Add(uinfo);
                }
            }
            //Get Tradelog
            u.Trades = await DbTradelog.GetUserTradeLog(u.UserInfo.Username);
            u.WonderTrades = await DbTradelog.GetUserWonderTradeLog(u.UserInfo.Username);

            //Get warnings
            const string warningCommand = "SELECT * FROM warnings WHERE user_id = @id";
            var j = new MySqlCommand(warningCommand, conn.Connection);
            j.Parameters.AddWithValue("id", u.UserInfo.User_Id);
            u.Warnings = new List<WarningsModel>();
            using (var res = await j.ExecuteReaderAsync())
            {
                while (await res.ReadAsync())
                {
                    var warn = new WarningsModel
                    {
                        Id = (uint) res["id"],
                        Reason = (string) res["reason"],
                        Time = (DateTime) res["time"],
                        UserId = u.UserInfo.User_Id,
                        Username = username
                    };
                    u.Warnings.Add(warn);
                }
            }

            //Get user notes
            const string notesCommand = "SELECT * FROM usernotes WHERE user_id = @id";
            var k = new MySqlCommand(notesCommand, conn.Connection);
            k.Parameters.AddWithValue("id", u.UserInfo.User_Id);
            u.Notes = new List<NotesModel>();
            using (var res = await k.ExecuteReaderAsync())
            {
                while (await res.ReadAsync())
                {
                    var note = new NotesModel
                    {
                        Moderator = (string) res["moderator"],
                        Time = (DateTime) res["time"],
                        Note = (string) res["note"]
                    };
                    u.Notes.Add(note);
                }
            }


            u.GTS = await DbGTS.GetUserGTS(u.UserInfo.User_Id);

            conn.Close();
            return u;
        }
        public static async Task<int> RegisterWebAdmin(string id, string name)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected)
            {
                const string com = "SELECT * FROM newwebadmin WHERE id = @param_val_1";
                var mCom = new MySqlCommand(com, conn.Connection);
                mCom.Parameters.AddWithValue("param_val_1", id);
                using (var res = await mCom.ExecuteReaderAsync())
                {
                    if (res.HasRows)
                    {
                        while (await res.ReadAsync())
                        {
                            var a = (int) res["access"];
                            conn.Close();
                            return a;
                        }
                    }
                }
                const string addcom = "INSERT INTO newwebadmin (id, name, access) VALUES (@param_val_1, @param_val_2, false)";
                var addCom = new MySqlCommand(addcom, conn.Connection);
                addCom.Parameters.AddWithValue("param_val_1", id);
                addCom.Parameters.AddWithValue("param_val_2", name);
                await addCom.ExecuteNonQueryAsync();
                conn.Close();
                return 0;
            }

            conn.Close();
            return 0;
        }
        public static async Task<string> GetUsernameFromId(uint id)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected)
            {
                const string com = "SELECT * FROM users WHERE user_id=@val";
                var m = new MySqlCommand(com, conn.Connection);
                m.Parameters.AddWithValue("val", id);
                using (var r = await m.ExecuteReaderAsync())
                {
                    var str = "";
                    while (await r.ReadAsync())
                    {

                        str = (string)r["username"];
                    }
                    conn.Close();
                    return str;
                }
            }
            conn.Close();
            return "";
        }
        public static async Task BanAccount(string username, bool newvalue)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected)
            {
                const string com = "UPDATE users SET banned= @value WHERE username = @parameter";
                var m = new MySqlCommand(com, conn.Connection);
                m.Parameters.AddWithValue("parameter", username);
                m.Parameters.AddWithValue("value", newvalue);
                await m.ExecuteNonQueryAsync();
            }
            conn.Close();
        }
        public static async Task BanIPs(uint id, bool value)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected)
            {
                const string com = "UPDATE ips SET ipban= @value WHERE ip IN (SELECT ip FROM (SELECT * FROM ips WHERE user_id = @parameter) AS ipsThing)";
                var m = new MySqlCommand(com, conn.Connection);
                m.Parameters.AddWithValue("parameter", id);
                m.Parameters.AddWithValue("value", value);
                await m.ExecuteNonQueryAsync();
            }
            conn.Close();
        }
        public static async Task BanSingleIp(string ip, bool value)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected)
            {
                const string com = "UPDATE ips SET ipban= @value WHERE ip = @parameter";
                var m = new MySqlCommand(com, conn.Connection);
                m.Parameters.AddWithValue("parameter", ip);
                m.Parameters.AddWithValue("value", value);
                await m.ExecuteNonQueryAsync();
            }
            conn.Close();
        }
        public static async Task BanAlts(uint id, bool value)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected)
            {
                const string com = "UPDATE users SET banned=@para WHERE user_id IN (SELECT user_id FROM ips WHERE ip IN (SELECT ip FROM ips WHERE user_id = @val))";
                var m = new MySqlCommand(com, conn.Connection);
                m.Parameters.AddWithValue("val", id);
                m.Parameters.AddWithValue("para", value);
                await m.ExecuteNonQueryAsync();
            }
            conn.Close();
        }

    }
    public class OpenConnection
    {
        private readonly string _connString =
            string.Format("Server=localhost; database={0}; UID={1}; password={2}; SslMode=none", DBAuth.Database,
                DBAuth.Username, DBAuth.Password);
        public readonly MySqlConnection Connection;
        public OpenConnection()
        {
            Connection = new MySqlConnection(_connString);
            Connection.Open();
        }
        public bool IsConnected => Connection.State == System.Data.ConnectionState.Open;
        public void Close()
        {
            Connection.Close();
        }
    }
}
