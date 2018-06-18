using System;
using InsurgenceServer.ClientHandler;

namespace InsurgenceServer.WonderTrade
{
    public class WonderTradeHolder
    {
        public DateTime Time { get; private set; }
        public Client Client { get; private set; }
        public GTS.GamePokemon Pokemon { get; private set; }

        public WonderTradeHolder(Client client, GTS.GamePokemon pokemon)
        {
            Time = DateTime.UtcNow;
            Client = client;
            Pokemon = pokemon;
        }
    }
}
