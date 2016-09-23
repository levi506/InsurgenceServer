using System;
using System.Threading;

namespace InsurgenceServer
{
	public class MainClass
	{
		public static void Main(string[] args)
		{
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            MainClass c = new MainClass();
            c.Begin();
		}
        public void Begin()
        {
            Console.WriteLine("Starting Server!");
            Console.WriteLine("Setting up tiers");
            Battles.Matchmaking.SetupTiers();
            Console.WriteLine("Setting up database!");
            Database.DbInit.Connect();
            Console.WriteLine("Reading datafiles!");
            GrowthRates.Read();

            new Thread(Logger.ErrorLog.Initialize
            ).Start();


            new Thread(Battles.RandomBattles.MatchRandoms
            ).Start();

            new Thread(ClientHandler.ClientChecker
            ).Start();

            new Thread(TradeHandler.TradeChecker
           ).Start();

            new Thread(BattleHandler.BattleChecker
           ).Start();

            new Thread(Battles.RandomBattles.CleanRandoms
           ).Start();

            new Thread(WonderTrade.WonderTradeHandler.Loop
           ).Start();

            new MainConnector();
            Console.ReadLine();
        }
        static string _lastError;
        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            if (((Exception)e.ExceptionObject).StackTrace != _lastError)
            {
                _lastError = ((Exception)e.ExceptionObject).StackTrace;
                try
                {
                    Logger.ErrorLog.Log((Exception)e.ExceptionObject);
                }
                catch
                {
                    // ignored
                }
                Console.WriteLine(e.ExceptionObject.ToString());
                Console.WriteLine("Press Enter to continue");
                Console.ReadLine();
            }
            
            Environment.Exit(1);
        }
    }
}
