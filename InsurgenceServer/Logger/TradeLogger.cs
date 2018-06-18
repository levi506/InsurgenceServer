using System;
using System.Threading.Tasks;

namespace InsurgenceServer.Logger
{
    public static class TradeLogger
    {
        public static async Task LogTrade(string user1, string user2, string pokemon1, string pokemon2)
        {
            try
            {
                await ActualLog(user1, user2, pokemon1, pokemon2);
            }
            catch (Exception e) {
                InsurgenceServer.Logger.ErrorLog.Log(e);
            }
        }
        private static async Task ActualLog(string user1, string user2, string pokemon1, string pokemon2)
        {
            var poke1Json = Utilities.Encoding.Base64Decode(pokemon1);
            var poke2Json = Utilities.Encoding.Base64Decode(pokemon2);
            await Database.DbTradelog.LogTrade(user1, user2, poke1Json, poke2Json);
            InsurgenceServer.Logger.Logger.Log($"Trade between: {user1} and {user2}");
        }

    }
}

