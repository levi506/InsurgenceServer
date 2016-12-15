using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminSiteNew.Database;
using Newtonsoft.Json;
using AdminSiteNew.Models;

namespace AdminSiteNew.Database
{
    internal static class Database
    {
        public static UserRequest GetUser(string username)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var u = new UserRequest();
                u.UserInfo = new UserInfo();
                u.UserInfo.Username = username;

                string usercommand = "SELECT user_id, banned, base FROM users WHERE username = @param_val_1;";
                MySqlCommand m = new MySqlCommand(usercommand, conn.Connection);
                m.Parameters.AddWithValue("@param_val_1", username);
                var result = m.ExecuteReader();
                if (!result.HasRows)
                {
                    conn.Close();
                    return null;
                }
                while (result.Read())
                {
                    u.UserInfo.User_Id = (uint)result["user_id"];
                    if (result["banned"].GetType() != typeof(DBNull))
                        u.UserInfo.Banned = (bool)result["banned"];
                    else
                        u.UserInfo.Banned = false;
                    u.FriendSafariString = (string)result["base"];
                }
                result.Close();
                u.FriendSafari = new FriendSafari(u.FriendSafariString);

                string getLoginData = "SELECT lastlogin FROM user_data WHERE user_id = @param_val_1";
                MySqlCommand glc = new MySqlCommand(getLoginData, conn.Connection);
                glc.Parameters.AddWithValue("@param_val_1", u.UserInfo.User_Id);
                var glcresult = glc.ExecuteReader();
                while (glcresult.Read())
                {
                    if (glcresult["lastlogin"].GetType() != typeof(DBNull))
                    {
                        u.UserInfo.LastLoggedIn = (DateTime)glcresult["lastlogin"];
                    }
                }
                glcresult.Close();
                //Find IPs----------------------------------
                string ipcommand = "SELECT ip, ipban FROM ips WHERE user_id = @param_val_1;";
                MySqlCommand n = new MySqlCommand(ipcommand, conn.Connection);
                n.Parameters.AddWithValue("@param_val_1", u.UserInfo.User_Id);
                var ipresult = n.ExecuteReader();
                u.IPs = new List<IPInfo>();
                while (ipresult.Read())
                {
                    bool ban = false;
                    if (ipresult["ipban"].GetType() != typeof(DBNull))
                    {
                        if (ipresult["ipban"].GetType() == typeof(sbyte))
                        {
                            ban = Convert.ToBoolean((sbyte)ipresult["ipban"]);
                        }
                        else
                        {
                            ban = (bool)ipresult["ipban"];
                        }
                    }
                    else
                    {
                        ban = false;
                    }
                    var ip = new IPInfo
                    {
                        IP = (string)ipresult["ip"],
                        Banned = ban
                    };
                    u.IPs.Add(ip);
                }
                ipresult.Close();
                //Find Alts----------------------------------
                string altipcommand = "SELECT user_id FROM ips WHERE FIND_IN_SET (ip, @param_val_1) != 0";
                MySqlCommand o = new MySqlCommand(altipcommand, conn.Connection);
                var ipl = new List<string>();
                foreach (var ip in u.IPs)
                {
                    ipl.Add(ip.IP);
                }
                o.Parameters.AddWithValue("param_val_1", string.Join(",", ipl));
                var l = new List<uint>();
                var altipresult = o.ExecuteReader();
                while (altipresult.Read())
                {
                    l.Add((uint)altipresult["user_id"]);
                }
                altipresult.Close();
                var ls = new List<string>();
                string altcommand = "SELECT username, user_id, banned FROM users WHERE FIND_IN_SET (user_id, @param_val_1) != 0";
                MySqlCommand p = new MySqlCommand(altcommand, conn.Connection);
                p.Parameters.AddWithValue("param_val_1", string.Join(",", l));
                u.Alts = new List<UserInfo>();
                var altresult = p.ExecuteReader();
                while (altresult.Read())
                {
                    var uinfo = new UserInfo
                    {
                        User_Id = (uint)altresult["user_id"],
                        Username = (string)altresult["username"],
                        Banned = (bool)altresult["banned"]
                    };
                    u.Alts.Add(uinfo);
                }
                conn.Close();
                //Get Tradelog
                u.Trades = DbTradelog.GetUserTradeLog(u.UserInfo.Username);
                return u;
            }
            conn.Close();
            return null;
        }
        public static int RegisterWebAdmin(string id, string name)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var com = "SELECT * FROM webadmin WHERE id = @param_val_1";
                MySqlCommand MCom = new MySqlCommand(com, conn.Connection);
                MCom.Parameters.AddWithValue("param_val_1", id);
                var res = MCom.ExecuteReader();
                if (!res.HasRows)
                {
                    res.Close();
                    var addcom = "INSERT INTO webadmin (id, name, access) VALUES (@param_val_1, @param_val_2, false)";
                    MySqlCommand AddCom = new MySqlCommand(addcom, conn.Connection);
                    AddCom.Parameters.AddWithValue("param_val_1", id);
                    AddCom.Parameters.AddWithValue("param_val_2", name);
                    AddCom.ExecuteNonQuery();
                    conn.Close();
                    return 0;
                }
                while (res.Read())
                {
                    return (int) res["access"];
                }
            }
            conn.Close();
            return 0;
        }
        public static string GetUsernameFromID(uint id)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var com = "SELECT * FROM users WHERE user_id=@val";
                MySqlCommand m = new MySqlCommand(com, conn.Connection);
                m.Parameters.AddWithValue("val", id);
                var r = m.ExecuteReader();
                string str = "";
                while (r.Read())
                {

                    str = (string)r["username"];
                }
                conn.Close();
                return str;
            }
            conn.Close();
            return "";
        }
        public static void BanAccount(string username, bool newvalue)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var com = "UPDATE users SET banned= @value WHERE username = @parameter";
                MySqlCommand m = new MySqlCommand(com, conn.Connection);
                m.Parameters.AddWithValue("parameter", username);
                m.Parameters.AddWithValue("value", newvalue);
                m.ExecuteNonQuery();
            }
            conn.Close();
        }
        public static void BanIPs(uint id, bool value)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var com = "UPDATE ips SET ipban= @value WHERE user_id = @parameter";
                MySqlCommand m = new MySqlCommand(com, conn.Connection);
                m.Parameters.AddWithValue("parameter", id);
                m.Parameters.AddWithValue("value", value);
                m.ExecuteNonQuery();
            }
            conn.Close();
        }
        public static void BanSingleIp(string ip, bool value)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var com = "UPDATE ips SET ipban= @value WHERE ip = @parameter";
                MySqlCommand m = new MySqlCommand(com, conn.Connection);
                m.Parameters.AddWithValue("parameter", ip);
                m.Parameters.AddWithValue("value", value);
                m.ExecuteNonQuery();
            }
            conn.Close();
        }
        public static void BanAlts(uint id, bool value)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
            {
                var com = "UPDATE users SET banned=@para WHERE user_id IN (SELECT user_id FROM ips WHERE ip IN (SELECT ip FROM ips WHERE user_id = @val))";
                var m = new MySqlCommand(com, conn.Connection);
                m.Parameters.AddWithValue("val", id);
                m.Parameters.AddWithValue("para", value);
                m.ExecuteNonQuery();
            }
            conn.Close();
        }

    }
    public class OpenConnection
    {
        private readonly string connstring = string.Format("Server=localhost; database={0}; UID={1}; password={2}", DBAuth.Database, DBAuth.Username , DBAuth.Password);
        public MySqlConnection Connection;
        public OpenConnection()
        {
            Connection = new MySqlConnection(connstring);
            Connection.Open();
        }
        public bool isConnected()
        {
            if (Connection.State == System.Data.ConnectionState.Open)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Close()
        {
            if (isConnected())
            {
                Connection.Close();
            }
        }
    }
}
