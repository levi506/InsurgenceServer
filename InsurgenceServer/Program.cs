using System;
using System.Threading;

namespace InsurgenceServer
{
	public class MainClass
	{
		public static void Main(string[] args)
		{
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            var c = new MainClass();
            c.Begin();
		}
        public void Begin()
        {
            Console.WriteLine("Starting Server!");
            Console.WriteLine("Creating Commands");
            NewCommandExecutor.Initialize();
            Console.WriteLine("Setting up tiers");
            Battles.Matchmaking.SetupTiers();
            Console.WriteLine("Setting up database!");
            Database.DBCreator.CreateTables();
            Database.DbInit.Connect();
            Console.WriteLine("Reading datafiles!");
            GrowthRates.Read();

            new Thread(Logger.ErrorLog.Initialize
            ).Start();


            Battles.RandomBattles.MatchRandoms();

#pragma warning disable 4014
            Checker.BeginChecking();
            WonderTrade.WonderTradeHandler.Loop();
#pragma warning restore 4014


            // ReSharper disable once ObjectCreationAsStatement
            new MainConnector();
            Console.ReadLine();
        }

	    private static string _lastError;

	    private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
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
            }
            
            Environment.Exit(1);
        }
    }
}
