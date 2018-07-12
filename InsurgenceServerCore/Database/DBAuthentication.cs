using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InsurgenceServerCore.ClientHandler;
using InsurgenceServerCore.Utilities;
using MySql.Data.MySqlClient;

namespace InsurgenceServerCore.Database
{
    public static class DbAuthentication
    {
        public static async Task<LoginResult> Login(string username, string password, Client client)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
            {
                await conn.Close();
                return LoginResult.WrongUsername;
            }
            username = username.RemoveSpecialCharacters();
            const string logincommand = "SELECT users.user_id, usergroup, banned, password, admin, " +
                                        "GROUP_CONCAT(ip separator ',') as 'IPs', " +
                                        "(SELECT COUNT(*) FROM ips WHERE ip=@param_val_2 AND ipban=1) ipbans " +
                                        "FROM users " +
                                        "INNER JOIN ips " +
                                        "ON users.user_id=ips.user_id " +
                                        "WHERE username = @param_val_1 ";
            var m = new MySqlCommand(logincommand, conn.Connection);
            m.Parameters.AddWithValue("@param_val_1", username);
            m.Parameters.AddWithValue("@param_val_2", client.Ip.ToString());
            var result = await m.ExecuteReaderAsync();
            var ret = LoginResult.Unset;
            if (!result.HasRows)
            {
                await conn.Close();
                return LoginResult.WrongUsername;
            }
            var ips = new List<string>();

            while (await result.ReadAsync())
            {
                if (result["password"].ToString() != password)
                {
                    await conn.Close();
                    return LoginResult.WrongPassword;
                }
                if (result["admin"] is DBNull)
                {
                    client.Admin = false;
                }
                else if ((bool)result["admin"])
                {
                    client.Admin = true;
                }

                if (client.Admin)
                {
                    Console.WriteLine("Admin logged in, username: " + username);
                }
                else if ((bool)result["banned"] && !client.Admin)
                {
                    await conn.Close();
                    return LoginResult.Banned;
                }
                if (((long)result["ipbans"] > 0))
                {
                    ret = LoginResult.IpBanned;
                }
                ips = result["IPs"].ToString().Split(',').ToList();

                var id = (uint)result["user_id"];
                client.UserId = id;
            }
            result.Close();

            if (!ips.Contains(client.Ip.ToString()))
            {
                const string ipsetcommand = "INSERT INTO ips VALUES (@param_val_1, @param_val_2, @param_val_3)";
                var o = new MySqlCommand(ipsetcommand, conn.Connection);
                o.Parameters.AddWithValue("@param_val_1", client.UserId);
                o.Parameters.AddWithValue("@param_val_2", client.Ip);
                o.Parameters.AddWithValue("@param_val_3", 0);
                try
                {
                    await o.ExecuteNonQueryAsync();
                }
                catch
                {
                    // ignore
                }
            }

            if (ret == LoginResult.Unset)
                ret = LoginResult.Okay;
            if (ret == LoginResult.Banned || ret == LoginResult.IpBanned)
            {
#pragma warning disable 4014
                DbUserManagement.Ban(client.UserId, ips);
                DBWarnLog.LogWarning(client.UserId, "Automatic ban: IP ban");
#pragma warning restore 4014
                Logger.Logger.Log($"User was banned automatically: {username}");
            }
            const string loggedInRegisterCom = "UPDATE user_data SET lastlogin = @param_val_2 WHERE user_id = @param_val_1";
            var logregcommand = new MySqlCommand(loggedInRegisterCom, conn.Connection);
            logregcommand.Parameters.AddWithValue("param_val_1", client.UserId);
            logregcommand.Parameters.AddWithValue("param_val_2", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            await logregcommand.ExecuteNonQueryAsync();

            if (ret == LoginResult.Okay)
            {
                Logger.Logger.Log($"User logged in: {username}");
            }

            await conn.Close();
            return ret;
        }

        public static async Task Register(Client client, string username, string password, string email)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
            {
                await conn.Close();
                return;
            }
            username = username.RemoveSpecialCharacters();

            var check = new MySqlCommand("SELECT " +
                                        "(SELECT COUNT(*) username FROM users WHERE username = @val) usernames, " +
                                        "(SELECT COUNT(*) FROM ips WHERE ip = @ip AND ipban = 1) ipbans", conn.Connection);
            check.Parameters.AddWithValue("val", username);
            check.Parameters.AddWithValue("ip", client.Ip.ToString());
            var checkres = await check.ExecuteReaderAsync();

            var canContinue = true;
            while (await checkres.ReadAsync())
            {
                if ((long) checkres["usernames"] > 0)
                {
                    await client.SendMessage("<REG result=0>");
                    canContinue = false;
                }
                else if ((long) checkres["ipbans"] > 0)
                {
                    await client.SendMessage("<LOG result=2>");
                    canContinue = false;
                    Logger.Logger.Log($"User was blocked from registering because of IP ban: {username}");
                }
            }

            if (!canContinue)
            {
                await conn.Close();
                return;
            }
            checkres.Close();

            var create = new MySqlCommand("INSERT INTO users (username, password, email, usergroup, base, sprite) " +
                "VALUES (@name, @pass, @email, @usergroup, @base, @sprite)",
                conn.Connection);
            create.Parameters.AddWithValue("name", username);
            create.Parameters.AddWithValue("pass", password);
            create.Parameters.AddWithValue("email", email);
            create.Parameters.AddWithValue("usergroup", 0);
            create.Parameters.AddWithValue("base", "");
            create.Parameters.AddWithValue("sprite", "");
            await create.ExecuteNonQueryAsync();

            var getUserid = new MySqlCommand("SELECT user_id FROM users WHERE username = @user", conn.Connection);
            getUserid.Parameters.AddWithValue("user", username);
            var idreader = await getUserid.ExecuteReaderAsync();
            while (await idreader.ReadAsync())
            {
                client.UserId = (uint)idreader["user_id"];
            }
            idreader.Close();

            var logonline = new MySqlCommand("INSERT INTO user_data (user_id, lastlogin) VALUES (@uid, @time)", conn.Connection);
            logonline.Parameters.AddWithValue("uid", client.UserId);
            logonline.Parameters.AddWithValue("time", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
#pragma warning disable 4014
            logonline.ExecuteNonQueryAsync();
#pragma warning restore 4014

            const string ipsetcommand = "INSERT INTO ips VALUES (@param_val_1, @param_val_2, @param_val_3)";
            var o = new MySqlCommand(ipsetcommand, conn.Connection);
            o.Parameters.AddWithValue("@param_val_1", client.UserId);
            o.Parameters.AddWithValue("@param_val_2", client.Ip);
            o.Parameters.AddWithValue("@param_val_3", 0);
#pragma warning disable 4014
            o.ExecuteNonQueryAsync();
#pragma warning restore 4014

            await client.SendMessage("<REG result=2>");
            await conn.Close();

            Logger.Logger.Log($"User registered: {username}");
        }
    }
    public enum LoginResult
    {
        Unset = -1, WrongUsername = 0, WrongPassword, Banned, IpBanned, Okay
    }
}
