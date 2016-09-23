using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading;

namespace InsurgenceServer
{
	public static class ClientHandler
	{
		public static List<Client> ActiveClients = new List<Client>();

		public static Client GetClient(string username)
		{
			var c = ActiveClients.Where(x => x.Username == username.ToLower());
			return c.FirstOrDefault();
		}
        public static Client GetClient(uint userId)
        {
            var c = ActiveClients.Where(x => x.UserId == userId);
            return c.FirstOrDefault();
        }
        public static List<Client> GetClientList(List<uint> userIds)
        {
            return ActiveClients.Where(x => userIds.Contains(x.UserId)).ToList();
        }

        public static void Remove(Client client)
		{
			try
			{
				ActiveClients.Remove(client);
			}
		    catch
		    {
		        // ignored
		    }
		}

		public static void ClientChecker()
		{
            try
            {
                while (Data.Running)
                {
                    for (var i = 0; i < ActiveClients.Count; i++)
                    {
                        try
                        {
                            if (i >= ActiveClients.Count)
                                continue;
                            var c = ActiveClients[i];
                            c.Ping();
                            if (c.ActualCient == null)
                            {
                                c.Disconnect();
                                continue;
                            }
                            if (!c.ActualCient.Connected)
                            {
                                c.Disconnect();
                                continue;
                            }
                            if ((DateTime.UtcNow - c.LastActive).TotalMinutes >= 5)
                            {
                                c.Disconnect();
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.ErrorLog.Log(e);
                            Console.WriteLine(e);
                        }
                    }
                    Thread.Sleep(5000);
                }
            }
            catch
            {
                ClientChecker();
            }
		}
	}
}

