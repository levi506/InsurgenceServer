using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace InsurgenceServer
{
	public class MainConnector
	{
		public MainConnector()
		{
            var ipstr = Data.Ip;
            var p = (int)Environment.OSVersion.Platform;
            //Set ip to 127.0.0.1 for Windows, for debugging purposes
            if (!((p == 4) || (p == 6) || (p == 128)))
            {
                ipstr = "127.0.0.1";
            }
            var ip = IPAddress.Parse(ipstr);
            Data.Server = new TcpListener(ip, Data.Port);
            Data.SiteServer = new TcpListener(IPAddress.Parse("127.0.0.1"), 6419);
			Data.Server.Start();
            Data.SiteServer.Start();
			Console.WriteLine($"Server Started on {ip}:{Data.Port}");

            new Thread(MainListener
            ).Start();
            new Thread(CommunicationListener
            ).Start();
        }
        private static void MainListener()
        {
            while (Data.Running)
            {
                try
                {
                    var client = Data.Server.AcceptTcpClient();
                    new Thread(() =>
                    {
                        // ReSharper disable once ObjectCreationAsStatement
                        new Client(client);
                    }).Start();
                }
                catch (Exception e)
                {
                    Logger.ErrorLog.Log(e);
                    Console.WriteLine(e);
                }

            }
        }
        private static void CommunicationListener()
        {
            while (Data.Running)
            {
                try
                {
                    var client = Data.SiteServer.AcceptTcpClient();
                    if ((((IPEndPoint)client.Client.RemoteEndPoint).Address).ToString() != "127.0.0.1")
                    {
                        continue;
                    }
                    new Thread(() =>
                    {
                        // ReSharper disable once ObjectCreationAsStatement
                        new SiteCommunication(client);
                    }).Start();
                }
                catch (Exception e)
                {
                    Logger.ErrorLog.Log(e);
                    Console.WriteLine(e);
                }
            }
        }
	}
}

