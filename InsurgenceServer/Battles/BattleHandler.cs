using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsurgenceServer
{
    public static class BattleHandler
    {
        public static List<Battle> ActiveBattles = new List<Battle>();

        public static Battle BeginBattle(string username, Client client, string trainer)
        {
            var u = username.ToLower();
            if (!Database.DBUserChecks.UserExists(username))
            {
                client.SendMessage(string.Format("<BAT user={0} result=0 trainer=nil>", username));
                return null;
            }
            if (Database.DBUserChecks.UserBanned(username))
            {
                client.SendMessage(string.Format("<BAT user={0} result=1 trainer=nil>", username));
                return null;
            }
            var c = ClientHandler.GetClient(u);
            if (c == null)
            {
                client.SendMessage(string.Format("<BAT user={0} result=2 trainer=nil>", username));
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
            foreach (var battle in ActiveBattles)
            {
                if (battle.username1 == u && battle.username2 == client.Username)
                {
                    return battle;
                }
            }
            return null;
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
                            continue;
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
