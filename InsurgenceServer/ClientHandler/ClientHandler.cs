using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InsurgenceServer.Utilities;

namespace InsurgenceServer.ClientHandler
{
    public static class ClientHandler
    {
        private const int ClientTimeout = 5;
        public static ConcurrentDictionary<Guid, Client> ActiveClients = new ConcurrentDictionary<Guid, Client>();

        public static Client GetClient(string username)
        {
            var c = ActiveClients.FirstOrDefault(x => x.Value.Username == username.ToLower().RemoveSpecialCharacters());
            return c.Equals(default(KeyValuePair<Guid, Client>)) ? null : c.Value;
        }
        public static Client GetClient(uint userId)
        {
            return ActiveClients.FirstOrDefault(x => x.Value.UserId == userId).Value;
        }
        public static List<Client> GetClientList(List<uint> userIds)
        {
            return userIds.Select(GetClient).ToList();
        }

        public static async Task Remove(Client client)
        {
            try
            {
                ActiveClients.TryRemove(client.Identifier, out _);
            }
            catch
            {
                // ignored
            }
        }

        public static async Task ClientChecker()
        {
            foreach (var keyValuePair in ActiveClients.ToList())
            {
                try
                {
                    var c = keyValuePair.Value;
                    if (c == null)
                        continue;
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

