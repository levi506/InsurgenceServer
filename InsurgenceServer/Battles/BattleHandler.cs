using System;
using System.Collections.Generic;
using System.Linq;

namespace InsurgenceServer
{
    public static class BattleHandler
    {
        public static List<Battle> ActiveBattles = new List<Battle>();

        public static Battle BeginBattle(string username, Client client, string trainer)
        {
            var u = username.ToLower();
            if (!Database.DbUserChecks.UserExists(username))
            {
                client.SendMessage($"<BAT user={username} result=0 trainer=nil>");
                return null;
            }
            if (Database.DbUserChecks.UserBanned(username))
            {
                client.SendMessage($"<BAT user={username} result=1 trainer=nil>");
                return null;
            }
            var c = ClientHandler.GetClient(u);
            if (c == null)
            {
                client.SendMessage($"<BAT user={username} result=2 trainer=nil>");
                return null;
            }
            var b = GetBattle(u, client);
            if (b == null)
                return new Battle(client, u, trainer);
            b.JoinBattle(client, trainer);
            return b;
        }

        public static Battle GetBattle(string username, Client client)
        {
            var u = username.ToLower();
            return ActiveBattles.FirstOrDefault(battle => battle.Username1 == u && battle.Username2 == client.Username);
        }

        public static void DeleteBattle(Battle b)
        {
            ActiveBattles.Remove(b);
        }

        public static void BattleChecker()
        {
            try
            {
                while (Data.Running)
                {
                    for (var i = 0; i < ActiveBattles.Count; i++)
                    {
                        if (i >= ActiveBattles.Count)
                            continue;
                        var t = ActiveBattles[i];
                        var timeactive = (DateTime.UtcNow - t.StartTime).TotalMinutes;
                        if (timeactive > 1 && t.Activated)
                        {
                            t.Kill();
                            continue;
                        }
                        if (timeactive >= 5)
                        {
                            t.Kill();
                        }
                    }
                    System.Threading.Thread.Sleep(5000);
                }
            }
            catch(Exception e)
            {
                Logger.ErrorLog.Log(e);
                Console.WriteLine(e);
                BattleChecker();
            }
        }
    }
}
