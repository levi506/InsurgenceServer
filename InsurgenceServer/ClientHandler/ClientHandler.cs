using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InsurgenceServer
{
    public static class ClientHandler
    {
        private const int ClientTimeout = 5;
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
            return ActiveClients.Where(x => userIds != null && userIds.Contains(x.UserId)).ToList();
        }

        public static async Task Remove(Client client)
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

        public static async Task ClientChecker()
        {
            for (var i = 0; i < ActiveClients.Count; i++)
            {
                try
                {
                    if (i >= ActiveClients.Count)
                        continue;
                    var c = ActiveClients[i];
                    await c.Ping();
                    if (c.ActualCient == null)
                    {
                        await c.Disconnect();
                        continue;
                    }
                    if (!c.ActualCient.Connected)
                    {
                        await c.Disconnect();
                        continue;
                    }
                    if ((DateTime.UtcNow - c.LastActive).TotalMinutes >= ClientTimeout)
                    {
                        await c.Disconnect();
                    }
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

