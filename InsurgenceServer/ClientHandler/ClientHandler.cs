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
        public static Client GetClient(uint User_id)
        {
            var c = ActiveClients.Where(x => x.User_Id == User_id);
            return c.FirstOrDefault();
        }
        public static List<Client> GetClientList(List<uint> user_ids)
        {
            return ActiveClients.Where(x => user_ids.Contains(x.User_Id)).ToList();
        }

        public static void Remove(Client client)
		{
			try
			{
				ActiveClients.Remove(client);
			}
			catch { }
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
                            if (c == null)
                            {
                                c.Disconnect();
                                continue;
                            }
                            if (c._client == null)
                            {
                                c.Disconnect();
                                continue;
                            }
                            if (!c._client.Connected)
                            {
                                c.Disconnect();
                                continue;
                            }
                            if ((DateTime.UtcNow - c.LastActive).TotalMinutes >= 5)
                            {
                                c.Disconnect();
                                continue;
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

