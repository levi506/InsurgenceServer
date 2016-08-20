using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsurgenceServer.Database
{
    public static class DBAuthentication
    {
        public static LoginResult Login(string username, string password, Client client)
        {
            var conn = new OpenConnection();
            if (conn.isConnected())
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
                uint id;
                while (result.Read())
                {
                    if (result["password"].ToString() != password)
                    {
                        conn.Close();
                        return LoginResult.WrongPassword;
                    }
                    if (result["admin"].GetType() == typeof(DBNull))
                    {
                        client.Admin = false;
                    }
                    else if (result["admin"].GetType() == typeof(byte))
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
                    id = (uint)result["user_id"];
                    client.User_Id = id;
                }
                result.Close();
                string ipCommand = "SELECT ip, ipban FROM ips WHERE user_id = @param_val_1";
                MySqlCommand n = new MySqlCommand(ipCommand, conn.Connection);
                n.Parameters.AddWithValue("@param_val_1", client.User_Id);
                var ipresult = n.ExecuteReader();
                List<string> ips = new List<string>();
                if (ipresult.HasRows)
                {
                    while (ipresult.Read())
                    {
                        if (ipresult["ipban"].GetType() != typeof(DBNull))
                        {
                            if (ipresult["ipban"].GetType() == typeof(bool))
                            {
                                var ipban = (bool)ipresult["ipban"];
                                if (ipban && !client.Admin)
                                {
                                    ret = LoginResult.IPBanned;
                                }
                            }
                            if (ipresult["ipban"].GetType() == typeof(SByte))
                            {
                                var ipban = Convert.ToBoolean((SByte)ipresult["ipban"]);
                                if (ipban && !client.Admin)
                                {
                                    ret = LoginResult.IPBanned;
                                }
                            }
                        }
                        ips.Add((string)ipresult["ip"]);
                    }
                }
                ipresult.Close();
                if (!ips.Contains(client.IP.ToString()))
                {
                    string ipsetcommand = "INSERT INTO ips VALUES (@param_val_1, @param_val_2, @param_val_3)";
                    MySqlCommand o = new MySqlCommand(ipsetcommand, conn.Connection);
                    o.Parameters.AddWithValue("@param_val_1", client.User_Id);
                    o.Parameters.AddWithValue("@param_val_2", client.IP);
                    o.Parameters.AddWithValue("@param_val_3", 0);
                    o.ExecuteNonQuery();
                }


                if (ret == LoginResult.Unset)
                    ret = LoginResult.Okay;
                if (ret == LoginResult.Banned || ret == LoginResult.IPBanned)
                {
                    Database.DBUserManagement.Ban(client.User_Id, ips);
                }
                string LoggedInRegisterCom = "UPDATE user_data SET lastlogin = @param_val_2 WHERE user_id = @param_val_1";
                MySqlCommand logregcommand = new MySqlCommand(LoggedInRegisterCom, conn.Connection);
                logregcommand.Parameters.AddWithValue("param_val_1", client.User_Id);
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
            if (conn.isConnected())
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
                    client.User_Id = (uint)idreader["user_id"];
                }
                idreader.Close();

                var logonline = new MySqlCommand("INSERT INTO user_data (user_id, lastlogin) VALUES (@uid, @time)", conn.Connection);
                logonline.Parameters.AddWithValue("uid", client.User_Id);
                logonline.Parameters.AddWithValue("time", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                logonline.ExecuteNonQuery();

                string ipsetcommand = "INSERT INTO ips VALUES (@param_val_1, @param_val_2, @param_val_3)";
                MySqlCommand o = new MySqlCommand(ipsetcommand, conn.Connection);
                o.Parameters.AddWithValue("@param_val_1", client.User_Id);
                o.Parameters.AddWithValue("@param_val_2", client.IP);
                o.Parameters.AddWithValue("@param_val_3", 0);
                o.ExecuteNonQuery();

                client.SendMessage("<REG result=2>");
            }
            conn.Close();
        }
    }
    public enum LoginResult
    {
        Unset = -1, WrongUsername = 0, WrongPassword, Banned, IPBanned, Okay
    }
}
