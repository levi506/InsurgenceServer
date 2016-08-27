using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsurgenceServer.WonderTrade
{
    public class WonderTradeHolder
    {
        public DateTime Time { get; private set; }
        public Client Client { get; private set; }
        public GTS.GamePokemon Pokemon { get; private set; }

        public WonderTradeHolder(Client _client, GTS.GamePokemon _pokemon)
        {
            Time = DateTime.UtcNow;
            Client = _client;
            Pokemon = _pokemon;
        }
    }
}
