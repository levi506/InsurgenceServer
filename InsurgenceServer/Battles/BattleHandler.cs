using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsurgenceServer
{
    public static class BattleHandler
    {
        private const int BattleTimeout = 5;
        public static List<Battle> ActiveBattles = new List<Battle>();

        public static async Task<Battle> BeginBattle(string username, Client client, string trainer)
        {
            var u = username.ToLower();
            if (!await Database.DbUserChecks.UserExists(username))
            {
                await client.SendMessage($"<BAT user={username} result=0 trainer=nil>");
                return null;
            }
            if (await Database.DbUserChecks.UserBanned(username))
            {
                await client.SendMessage($"<BAT user={username} result=1 trainer=nil>");
                return null;
            }
            var c = ClientHandler.GetClient(u);
            if (c == null)
            {
                await client.SendMessage($"<BAT user={username} result=2 trainer=nil>");
                return null;
            }
            var b = GetBattle(u, client);
            if (b == null)
                return new Battle(client, u, trainer);
            await b.JoinBattle(client, trainer);
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

        public static async Task BattleChecker()
        {
            for (var i = 0; i < ActiveBattles.Count; i++)
            {
                if (i >= ActiveBattles.Count)
                    continue;
                var t = ActiveBattles[i];
                var timeactive = (DateTime.UtcNow - t.StartTime).TotalMinutes;
                if (timeactive > 1 && t.Activated)
                {
                    await t.Kill();
                    continue;
                }
                if (timeactive >= BattleTimeout)
                {
                    await t.Kill();
                }
            }
        }
    }
}
