using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace InsurgenceServer.Database
{
    public static class DbAuthentication
    {
        public static LoginResult Login(string username, string password, Client client)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected())
            {
                string logincommand = "SELECT user_id, usergroup, banned, password, admin FROM users WHERE username = @param_val_1;";
                MySqlCommand m = new MySqlCommand(logincommand, conn.Connection);
                m.Parameters.AddWithValue("@param_val_1", username);
                var result = m.ExecuteReader();
                LoginResult ret = LoginResult.Unset;
                if (!result.HasRows)
                {
                    conn.Close();
                    return LoginResult.WrongUsername;
                }
                while (result.Read())
                {
                    if (result["password"].ToString() != password)
                    {
                        conn.Close();
                        return LoginResult.WrongPassword;
                    }
                    if (result["admin"] is DBNull)
                    {
                        client.Admin = false;
                    }
                    else if (result["admin"] is byte)
                    {
                        client.Admin = Convert.ToBoolean((byte)result["admin"]);
                    }
                    else
                    {
                        if ((bool)result["admin"])
                        {
                            client.Admin = true;
                        }
                    }
                    if (client.Admin)
                    {
                        Console.WriteLine("Admin logged in, username: " + username);
                    }
                    else if ((bool)result["banned"] && !client.Admin)
                    {
                        conn.Close();
                        return LoginResult.Banned;
                    }
                    var id = (uint)result["user_id"];
                    client.UserId = id;
                }
                result.Close();
                string ipCommand = "SELECT ip, ipban FROM ips WHERE user_id = @param_val_1";
                MySqlCommand n = new MySqlCommand(ipCommand, conn.Connection);
                n.Parameters.AddWithValue("@param_val_1", client.UserId);
                var ipresult = n.ExecuteReader();
                List<string> ips = new List<string>();
                if (ipresult.HasRows)
                {
                    while (ipresult.Read())
                    {
                        if (ipresult["ipban"].GetType() != typeof(DBNull))
                        {
                            if (ipresult["ipban"] is bool)
                            {
                                var ipban = (bool)ipresult["ipban"];
                                if (ipban && !client.Admin)
                                {
                                    ret = LoginResult.IpBanned;
                                }
                            }
                            if (ipresult["ipban"] is sbyte)
                            {
                                var ipban = Convert.ToBoolean((SByte)ipresult["ipban"]);
                                if (ipban && !client.Admin)
                                {
                                    ret = LoginResult.IpBanned;
                                }
                            }
                        }
                        ips.Add((string)ipresult["ip"]);
                    }
                }
                ipresult.Close();
                if (!ips.Contains(client.Ip.ToString()))
                {
                    string ipsetcommand = "INSERT INTO ips VALUES (@param_val_1, @param_val_2, @param_val_3)";
                    MySqlCommand o = new MySqlCommand(ipsetcommand, conn.Connection);
                    o.Parameters.AddWithValue("@param_val_1", client.UserId);
                    o.Parameters.AddWithValue("@param_val_2", client.Ip);
                    o.Parameters.AddWithValue("@param_val_3", 0);
                    o.ExecuteNonQuery();
                }


                if (ret == LoginResult.Unset)
                    ret = LoginResult.Okay;
                if (ret == LoginResult.Banned || ret == LoginResult.IpBanned)
                {
                    DbUserManagement.Ban(client.UserId, ips);
                }
                string loggedInRegisterCom = "UPDATE user_data SET lastlogin = @param_val_2 WHERE user_id = @param_val_1";
                MySqlCommand logregcommand = new MySqlCommand(loggedInRegisterCom, conn.Connection);
                logregcommand.Parameters.AddWithValue("param_val_1", client.UserId);
                logregcommand.Parameters.AddWithValue("param_val_2", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                logregcommand.ExecuteNonQuery();

                conn.Close();
                return ret;
            }
            else
            {
                conn.Close();
                return LoginResult.WrongUsername;
            }
        }
        public static void Register(Client client, string username, string password, string email)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected())
            {
                var check = new MySqlCommand("SELECT username FROM users WHERE username = @val", conn.Connection);
                check.Parameters.AddWithValue("val", username);
                var checkres = check.ExecuteReader();
                if (checkres.HasRows)
                {
                    client.SendMessage("<REG result=0>");
                    conn.Close();
                    return;
                }
                checkres.Close();
                var checke = new MySqlCommand("SELECT username FROM users WHERE email = @val", conn.Connection);
                checke.Parameters.AddWithValue("val", email);
                var checkrese = checke.ExecuteReader();
                if (checkrese.HasRows)
                {
                    client.SendMessage("<REG result=1>");
                    conn.Close();
                    return;
                }
                checkrese.Close();

                var ipban = new MySqlCommand("SELECT COUNT(*) FROM ips WHERE ip = @ip AND ipban = 1", conn.Connection);
                ipban.Parameters.AddWithValue("ip", client.Ip.ToString());
                var ipbancount = int.Parse(ipban.ExecuteScalar().ToString());
                if (ipbancount > 0)
                {
                    client.SendMessage("<LOG result=2>");
                    conn.Close();
                    return;
                }

                var create = new MySqlCommand("INSERT INTO users (username, password, email, usergroup, base, sprite) " + 
                    "VALUES (@name, @pass, @email, @usergroup, @base, @sprite)", 
                    conn.Connection);
                create.Parameters.AddWithValue("name", username);
                create.Parameters.AddWithValue("pass", password);
                create.Parameters.AddWithValue("email", email);
                create.Parameters.AddWithValue("usergroup", 0);
                create.Parameters.AddWithValue("base", "");
                create.Parameters.AddWithValue("sprite", "");
                create.ExecuteNonQuery();

                var getUserid = new MySqlCommand("SELECT user_id FROM users WHERE username = @user", conn.Connection);
                getUserid.Parameters.AddWithValue("user", username);
                var idreader = getUserid.ExecuteReader();
                while (idreader.Read())
                {
                    client.UserId = (uint)idreader["user_id"];
                }
                idreader.Close();

                var logonline = new MySqlCommand("INSERT INTO user_data (user_id, lastlogin) VALUES (@uid, @time)", conn.Connection);
                logonline.Parameters.AddWithValue("uid", client.UserId);
                logonline.Parameters.AddWithValue("time", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                logonline.ExecuteNonQuery();

                string ipsetcommand = "INSERT INTO ips VALUES (@param_val_1, @param_val_2, @param_val_3)";
                MySqlCommand o = new MySqlCommand(ipsetcommand, conn.Connection);
                o.Parameters.AddWithValue("@param_val_1", client.UserId);
                o.Parameters.AddWithValue("@param_val_2", client.Ip);
                o.Parameters.AddWithValue("@param_val_3", 0);
                o.ExecuteNonQuery();

                client.SendMessage("<REG result=2>");
            }
            conn.Close();
        }
    }
    public enum LoginResult
    {
        Unset = -1, WrongUsername = 0, WrongPassword, Banned, IpBanned, Okay
    }
}
