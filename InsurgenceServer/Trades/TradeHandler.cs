using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsurgenceServer
{
    public static class TradeHandler
    {
        private const int TradeTimeout = 5;
        public static List<Trade> ActiveTrades = new List<Trade>();

        public static async Task<Trade> BeginTrade(string username, Client client)
        {
            try
            {
                var u = username.ToLower();
                if (!await Database.DbUserChecks.UserExists(username))
                {
                    await client.SendMessage($"<TRA user={username} result=0>");
                    return null;
                }
                if (await Database.DbUserChecks.UserBanned(username))
                {
                    await client.SendMessage($"<TRA user={username} result=1>");
                    return null;
                }
                var c = ClientHandler.GetClient(u);
                if (c == null)
                {
                    await client.SendMessage($"<TRA user={username} result=2>");
                    return null;
                }
                if ((c.Ip.Equals(client.Ip)) && !client.Admin)
                {
                    await client.SendMessage("<GLOBAL message=You can not trade with the same IP>");
                    await client.SendMessage("<TRA dead>");
                    return null;
                }
                var t = await GetTrade(u, client);
                if (t == null)
                    return new Trade(client, u);
                await t.JoinTrade(client);
                return t;
            }
            catch(Exception e)
            {
                Logger.ErrorLog.Log(e);
                Console.WriteLine(e);
                return null;
            }
        }
        public static async Task<Trade> GetTrade(string username, Client client)
        {
            var u = username.ToLower();
            return ActiveTrades.FirstOrDefault(trade => trade.Username1 == u && trade.Username2 == client.Username);
        }
        public static async Task DeleteTrade(Trade trade)
        {
            try
            {
                ActiveTrades.Remove(trade);
            }
            catch
            {
                // ignored
            }
        }

        public static async Task TradeChecker()
        {
            for (var i = 0; i < ActiveTrades.Count; i++)
            {
                if (i >= ActiveTrades.Count)
                    continue;
                var t = ActiveTrades[i];
                var timeactive = (DateTime.UtcNow - t.StartTime).TotalMinutes;
                if (timeactive >= 1 && t.Activated)
                {
                    await t.Kill();
                    continue;
                }
                if (timeactive >= TradeTimeout)
                {
                    await t.Kill();
                    continue;
                }
                if (t.Client1 == null)
                {
                    await t.Kill();
                    continue;
                }
                if (t.Client2 == null && t.Activated)
                {
                    await t.Kill();
                }
            }
        }
    }

}

