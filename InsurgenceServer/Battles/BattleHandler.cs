using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using InsurgenceServer.ClientHandler;

namespace InsurgenceServer.Battles
{
    public static class BattleHandler
    {
        private const int BattleTimeout = 15;
        public static ConcurrentDictionary<Guid, Battle> ActiveBattles = new ConcurrentDictionary<Guid, Battle>();

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
            var c = ClientHandler.ClientHandler.GetClient(u);
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

        public static Battle GetBattle(Client client)
        {
            return ActiveBattles.ContainsKey(client.ActiveBattleId) ? ActiveBattles[client.ActiveBattleId] : null;
        }

        public static Battle GetBattle(string username, Client client)
        {
            var u = username.ToLower();
            var b = ActiveBattles.FirstOrDefault(battle =>
                battle.Value.Username1 == u && battle.Value.Username2 == client.Username).Value;
            if (b == null)
            {
                b = ActiveBattles.FirstOrDefault(battle =>
                    battle.Value.Username2 == u && battle.Value.Username1 == client.Username).Value;
            }
            return b;
        }

        public static void DeleteBattle(Battle b)
        {
            ActiveBattles.TryRemove(b.Id, out b);
        }

        public static async Task BattleChecker()
        {
            foreach (var activeBattle in ActiveBattles.ToArray())
            {
                var timeactive = (DateTime.UtcNow - activeBattle.Value.StartTime).TotalMinutes;
                if (timeactive > 1 && !activeBattle.Value.Activated)
                {
                    await activeBattle.Value.Kill("Took too long to respond");
                    continue;
                }
                var lastMessageMinutes = (DateTime.UtcNow - activeBattle.Value.LastMessageTime).TotalMinutes;
                if (lastMessageMinutes > 2)
                {
                    await activeBattle.Value.Kill("Battle timed out due to now messages");
                }
            }
        }
    }
}
