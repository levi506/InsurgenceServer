using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace InsurgenceServerCore.ClientHandler
{
    public class MainConnector
    {
        public MainConnector()
        {
#if DEBUG
            const string ipstr = "127.0.0.1";
    #else
            const string ipstr = Data.Ip;
#endif
            var ip = IPAddress.Parse(ipstr);
            Data.Server = new TcpListener(ip, Data.Port);
            Data.SiteServer = new TcpListener(IPAddress.Parse("127.0.0.1"), 6418);
            Data.Server.Start();
            Data.SiteServer.Start();
            Console.WriteLine($"Server Started on {ip}:{Data.Port}. Version : {Data.ServerVersion}");

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
                        try
                        {
                            new Client(client);
                        }
                        catch
                        {
                            // ignored
                        }
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
                    Console.WriteLine("Attempted connection admin site");
                    if ((((IPEndPoint)client.Client.RemoteEndPoint).Address).ToString() != "127.0.0.1")
                    {
                        Console.WriteLine("Not local");
                        client.Close();
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

