using System;
using System.Collections.Generic;
using System.Linq;

namespace InsurgenceServer
{
	public static class TradeHandler
	{
		public static List<Trade> ActiveTrades = new List<Trade>();

		public static Trade BeginTrade(string username, Client client)
		{
            try
            {
                var u = username.ToLower();
                if (!Database.DBUserChecks.UserExists(username))
                {
                    client.SendMessage(string.Format("<TRA user={0} result=0>", username));
                    return null;
                }
                if (Database.DBUserChecks.UserBanned(username))
                {
                    client.SendMessage(string.Format("<TRA user={0} result=1>", username));
                    return null;
                }
                var c = ClientHandler.GetClient(u);
                if (c == null)
                {
                    client.SendMessage(string.Format("<TRA user={0} result=2>", username));
                    return null;
                }
                if ((c.IP.ToString() == client.IP.ToString()) && !client.Admin)
                {
                    client.SendMessage(string.Format("<GLOBAL message=You can not trade with the same IP>"));
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
			foreach (var trade in ActiveTrades)
			{
				if (trade.username1 == u && trade.username2 == client.Username)
				{
					return trade;
				}
			}
			return null;
		}
		public static void DeleteTrade(Trade trade)
		{
            try
            {
                ActiveTrades.Remove(trade);
            }
            catch { }
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
                            return;
                        var t = ActiveTrades[i];
                        var timeactive = (DateTime.UtcNow - t.StartTime).TotalMinutes;
                        if (timeactive >= 1 && t.Activated)
                        {
                            t.Kill();
                            return;
                        }
                        if (timeactive >= 5)
                        {
                            t.Kill();
                            return;
                        }
                        if (t.Client1 == null)
                        {
                            t.Kill();
                            return;
                        }
                        if (t.Client2 == null && t.Activated)
                        {
                            t.Kill();
                            return;
                        }
                    }
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
        public string user1 { get; set; }
        public string user2 { get; set; }
        public Pokemon Pokemon1 { get; set; }
        public Pokemon Pokemon2 { get; set; }
    }

}

