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
			IPAddress ip = IPAddress.Parse(Data.IP);
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
                    Console.WriteLine(e);
                }

            }
		}
	}
}

