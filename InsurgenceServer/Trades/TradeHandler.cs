using System;
using System.Collections.Generic;
using System.Linq;

namespace InsurgenceServer
{
	public static class TradeHandler
	{
        private const int TradeTimeout = 5;
        public static List<Trade> ActiveTrades = new List<Trade>();

		public static Trade BeginTrade(string username, Client client)
		{
            try
            {
                var u = username.ToLower();
                if (!Database.DbUserChecks.UserExists(username))
                {
                    client.SendMessage($"<TRA user={username} result=0>");
                    return null;
                }
                if (Database.DbUserChecks.UserBanned(username))
                {
                    client.SendMessage($"<TRA user={username} result=1>");
                    return null;
                }
                var c = ClientHandler.GetClient(u);
                if (c == null)
                {
                    client.SendMessage($"<TRA user={username} result=2>");
                    return null;
                }
                if ((c.Ip.Equals(client.Ip)) && !client.Admin)
                {
                    client.SendMessage("<GLOBAL message=You can not trade with the same IP>");
                    client.SendMessage("<TRA dead>");
                    return null;
                }
                var t = GetTrade(u, client);
                if (t == null)
                    return new Trade(client, u);
                t.JoinTrade(client);
                return t;
            }
            catch(Exception e)
            {
                Logger.ErrorLog.Log(e);
                Console.WriteLine(e);
                return null;
            }
		}
		public static Trade GetTrade(string username, Client client)
		{
			var u = username.ToLower();
		    return ActiveTrades.FirstOrDefault(trade => trade.Username1 == u && trade.Username2 == client.Username);
		}
		public static void DeleteTrade(Trade trade)
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

        public static void TradeChecker()
        {
            try
            {
                while (Data.Running)
                {
                    for (var i = 0; i < ActiveTrades.Count; i++)
                    {
                        if (i >= ActiveTrades.Count)
                            continue;
                        var t = ActiveTrades[i];
                        var timeactive = (DateTime.UtcNow - t.StartTime).TotalMinutes;
                        if (timeactive >= 1 && t.Activated)
                        {
                            t.Kill();
                            continue;
                        }
                        if (timeactive >= TradeTimeout)
                        {
                            t.Kill();
                            continue;
                        }
                        if (t.Client1 == null)
                        {
                            t.Kill();
                            continue;
                        }
                        if (t.Client2 == null && t.Activated)
                        {
                            t.Kill();
                        }
                    }
                    System.Threading.Thread.Sleep(5000);
                }
            }
            catch (Exception e)
            {
                Logger.ErrorLog.Log(e);
                Console.WriteLine(e);
                TradeChecker();
            }
        }
	}
    public class TradeLog
    {
        public string User1 { get; set; }
        public string User2 { get; set; }
        public Pokemon Pokemon1 { get; set; }
        public Pokemon Pokemon2 { get; set; }
    }

}

