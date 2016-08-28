﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsurgenceServer.WonderTrade
{
    public static class WonderTradeHandler
    {
        public static List<WonderTradeHolder> List = new List<WonderTradeHolder>();

        public static void Loop()
        {
            while (Data.Running)
            {
                foreach (var trade in List.ToList())
                {
                    Console.WriteLine(trade.Pokemon.name);
                    try
                    {
                        if ((DateTime.UtcNow - trade.Time).TotalSeconds >= 60)
                        {
                            //Delete from list, send timeout message back to client
                            List.Remove(trade);
                            trade.Client.SendMessage("<WTRESULT result=1 user=nil pkmn=nil>");
                        }
                        if (trade.Client == null || !trade.Client.Connected)
                        {
                            //Delete from list
                            List.Remove(trade);
                        }
                    }
                    catch
                    {

                    }
                }
                try
                {
                    while (List.Count >= 2)
                    {
                        //Get 2 random entries
                        var r = new Random();
                        var i1 = r.Next(0, List.Count);
                        int i2 = r.Next(0, List.Count);
                        //We don't want two the same entries
                        while(i1 == i2)
                        {
                            //Break this if we don't have 2 entries anymore
                            if (List.Count < 2)
                                continue;
                            i2 = r.Next(0, List.Count);
                        }
                        var trade1 = List[i1];
                        var trade2 = List[i2];
                        //If either of the clients is not connected anymore, try looping again
                        if (!trade1.Client.Connected || !trade2.Client.Connected)
                            continue;
                        //If two ips are the same and neither is an admin, try looping again
                        if ((trade1.Client.IP == trade2.Client.IP) && (!trade1.Client.Admin || !trade2.Client.Admin))
                            continue;

                        //Execute trade, remove entries
                        ExecuteTrade(trade1.Client, trade2.Client, trade1.Pokemon, trade2.Pokemon);
                        List.Remove(trade1);
                        List.Remove(trade2);
                    }
                }
                catch
                {

                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        public static void ExecuteTrade(Client client1, Client client2, GTS.GamePokemon pkmn1, GTS.GamePokemon pkmn2)
        {
            var s1 = Utilities.Encoding.Base64Encode(JsonConvert.SerializeObject(pkmn1));
            var s2 = Utilities.Encoding.Base64Encode(JsonConvert.SerializeObject(pkmn2));

            client1.SendMessage(string.Format("<WTRESULT result=2 user={0} pkmn={1}>", client2.Username, pkmn2));
            client2.SendMessage(string.Format("<WTRESULT result=2 user={0} pkmn={1}>", client1.Username, pkmn1));
        }
        
        public static void DeleteFromClient(Client c)
        {
            foreach (var trade in List)
            {
                try
                {
                    if (trade.Client == c)
                    {
                        List.Remove(trade);
                    }
                }
                catch { }
            }
        }

        public static void AddTrade(Client client, string EncodedPkmns)
        {
            if (Database.DBUserChecks.UserBanned(client.Username))
            {
                client.SendMessage("<WTRESULT reult=0 user=nil pkmn=nil>");
                return;
            }
            if (List.FindAll(x => x.Client == client).Count > 0)
            {
                return;
            }

            var pkmn = JsonConvert.DeserializeObject<GTS.GamePokemon>(Utilities.Encoding.Base64Decode(EncodedPkmns));
            List.Add(new WonderTradeHolder(client, pkmn));
        }

        public static void CancelTrade(Client client)
        {
            DeleteFromClient(client);
        }
    }
}