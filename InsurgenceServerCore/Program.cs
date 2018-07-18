using System;
using InsurgenceServerCore.ClientHandler;

namespace InsurgenceServerCore
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
            var runTimeInfo = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.RuntimeFramework;
            Console.WriteLine($"Running with {runTimeInfo.FullName}");

            Console.WriteLine("Creating Commands");
            NewCommandExecutor.Initialize();
            Console.WriteLine("Setting up tiers");
#pragma warning disable 4014
            Battles.Matchmaking.SetupTiers();
#pragma warning restore 4014
            Console.WriteLine("Setting up database!");
            try
            {
                Database.DBCreator.CreateTables();
                Database.DbInit.Connect();
            }
            catch
            {

            }
            Console.WriteLine("Reading datafiles!");
            GrowthRates.Read();


#pragma warning disable 4014
            Battles.RandomBattles.MatchRandoms();
#pragma warning restore 4014

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
