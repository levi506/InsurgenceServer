using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace InsurgenceServer
{
	public class MainClass
	{
		public static void Main(string[] args)
		{
            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            MainClass c = new MainClass();
            c.Begin();
		}
        public void Begin()
        {
            Console.WriteLine("Starting Server!");
            Console.WriteLine("Setting up tiers");
            Battles.Matchmaking.SetupTiers();
            Console.WriteLine("Setting up database!");
            Database.DBInit.Connect();

            new Thread(() =>
               Logger.ErrorLog.Initialize()
            ).Start();


            new Thread(() =>
                Battles.RandomBattles.MatchRandoms()
            ).Start();

            new Thread(() =>
                ClientHandler.ClientChecker()
            ).Start();

            new Thread(() =>
               TradeHandler.TradeChecker()
           ).Start();

            new Thread(() =>
               BattleHandler.BattleChecker()
           ).Start();

            new Thread(() =>
               Battles.RandomBattles.CleanRandoms()
           ).Start();

            new MainConnector();
            Console.ReadLine();
        }
        static string LastError;
        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            if (((Exception)e.ExceptionObject).StackTrace == LastError)
            {
                LastError = ((Exception)e.ExceptionObject).StackTrace;
                try
                {
                    Logger.ErrorLog.Log((Exception)e.ExceptionObject);
                }
                catch { }
                Console.WriteLine(e.ExceptionObject.ToString());
                Console.WriteLine("Press Enter to continue");
                Console.ReadLine();
            }
            
            Environment.Exit(1);
        }
    }
}
