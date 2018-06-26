using System;
using InsurgenceServerCore.ClientHandler;

namespace InsurgenceServerCore.WonderTrade
{
    public class WonderTradeHolder
    {
        public Guid Id { get; private set; }
        public DateTime Time { get; private set; }
        public Client Client { get; private set; }
        public GTS.GamePokemon Pokemon { get; private set; }

        public WonderTradeHolder(Client client, GTS.GamePokemon pokemon)
        {
            Id = Guid.NewGuid();
            Time = DateTime.UtcNow;
            Client = client;
            Pokemon = pokemon;
        }
    }
}
