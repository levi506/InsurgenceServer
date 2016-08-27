using Newtonsoft.Json;
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
                foreach (var trade in List)
                {
                    try
                    {
                        if ((DateTime.UtcNow - trade.Time).TotalSeconds >= 60)
                        {
                            //Delete from list, send timeout message back to client
                        }
                        if (trade.Client != null || !trade.Client.Connected)
                        {
                            //Delete from list
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
                        var r = new Random();
                        var i1 = r.Next(0, List.Count);
                        int i2 = r.Next(0, List.Count);
                        while(i1 == i2)
                        {
                            if (List.Count < 2) continue;
                            i2 = r.Next(0, List.Count);
                        }
                        var trade1 = List[i1];
                        var trade2 = List[i2];
                        if (!trade1.Client.Connected || !trade2.Client.Connected)
                            continue;
                        if ((trade1.Client.IP == trade2.Client.IP) && (!trade1.Client.Admin || !trade2.Client.Admin))
                            continue;

                        ExecuteTrade(trade1.Client, trade2.Client, trade1.Pokemon, trade2.Pokemon);
                        List.Remove(trade1);
                        List.Remove(trade2);
                    }
                }
                catch
                {

                }
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
            var pkmn = JsonConvert.DeserializeObject<GTS.GamePokemon>(Utilities.Encoding.Base64Decode(EncodedPkmns));
            List.Add(new WonderTradeHolder(client, pkmn));
        }
    }
}
