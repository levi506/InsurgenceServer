using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InsurgenceServer
{
	public static class TradeLogger
	{
		public static int CurrentLogging = 0;

		public static void LogTrade(string user1, string user2, string pokemon1, string pokemon2)
		{
            //We fire the log function once then forget about it, so it acts completely non blocking
			#pragma warning disable 4014
			Task.Run(() =>
			{
				try
				{
					ActualLog(user1, user2, pokemon1, pokemon2);
				}
				catch (Exception e) {
                    Logger.ErrorLog.Log(e);
                    Console.WriteLine(e); }
			}).ConfigureAwait(false);
			#pragma warning restore 4014
		}
		private static void ActualLog(string user1, string user2, string pokemon1, string pokemon2)
		{
			CurrentLogging++;
			//byte[] data1 = Convert.FromBase64String(pokemon1);
			//byte[] data2 = Convert.FromBase64String(pokemon2);
            //var poke1 = Marshal.MarshalLoadPokemon(data1);
            //var poke2 = Marshal.MarshalLoadPokemon(data2);
            var poke1json = Utilities.Encoding.Base64Decode(pokemon1);
            var poke2json = Utilities.Encoding.Base64Decode(pokemon2);
            Database.DBTradelog.LogTrade(user1, user2, poke1json, poke2json);
			CurrentLogging--;
		}

	}
}

