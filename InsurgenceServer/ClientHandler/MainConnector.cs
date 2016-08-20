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
            var ipstr = Data.IP;
            int p = (int)Environment.OSVersion.Platform;
            //Set ip to 127.0.0.1 for Windows, for debugging purposes
            if (!((p == 4) || (p == 6) || (p == 128)))
            {
                ipstr = "127.0.0.1";
            }
            IPAddress ip = IPAddress.Parse(ipstr);
            Data.Server = new TcpListener(ip, Data.Port);
			Data.Server.Start();
			Console.WriteLine(string.Format("Server Started on {0}:{1}", ip, Data.Port));

			while (Data.Running)
			{
                try
                {
                    TcpClient client = Data.Server.AcceptTcpClient();
                    new Thread(() =>
                        new Client(client)
                    ).Start();
                }
                catch(Exception e)
                {
                    Logger.ErrorLog.Log(e);
                    Console.WriteLine(e);
                }

            }
		}
	}
}

