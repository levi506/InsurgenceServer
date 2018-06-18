using System;
using System.Threading.Tasks;
using InsurgenceServer.ClientHandler;
using InsurgenceServer.Logger;
using Newtonsoft.Json;

namespace InsurgenceServer.Trades
{
    public class Trade
    {
        public readonly string Username1;
        public readonly string Username2;

        public readonly Client Client1;
        public Client Client2;

        public bool Activated { get; private set; }
        public DateTime StartTime;

        public Trade(Client client, string username)
        {
            Username1 = client.Username;
            Username2 = username;
            Client1 = client;
            StartTime = DateTime.UtcNow;
            TradeHandler.ActiveTrades.Add(this);

        }
        public async Task JoinTrade(Client client)
        {
            Activated = true;
            Client2 = client;
            await Client1.SendMessage($"<TRA user={Username2} result=4>");
            await Client2.SendMessage($"<TRA user={Username1} result=4>");
        }
        public async Task ConfirmStart(string username)
        {
            if (username == Username1)
            {
                await Client2.SendMessage("<TRA start>");
            }
            else {
                await Client1.SendMessage("<TRA start>");
            }
        }
        public async Task SendParties(string username, string party)
        {
            if (username == Username1)
            {
                await Client2.SendMessage($"<TRA party={party}>");
            }
            else {
                await Client1.SendMessage($"<TRA party={party}>");
            }
        }
        public async Task Kill()
        {
            if (Client1 != null && Client1.Connected)
                await Client1.SendMessage("<TRA dead>");
            if (Client2 != null && Client2.Connected)
                await Client2.SendMessage("<TRA dead>");
            if (Client1 != null)
                Client1.ActiveTrade = null;
            if (Client2 != null)
                Client2.ActiveTrade = null;
            await TradeHandler.DeleteTrade(this);
        }
        private string _client1Pokemon;
        private string _client2Pokemon;
        public async Task Offer(Client client, string username, string offer)
        {
            var offerJson = Utilities.Encoding.Base64Decode(offer);
            offerJson = offerJson.Replace(",,", ",");
            offer = Utilities.Encoding.Base64Encode(offerJson);

            var pkmn = JsonConvert.DeserializeObject<GTS.GamePokemon>(offerJson);
            if (! await TradeValidator.IsPokemonValid(pkmn, client.UserId))
            {
                await Kill();
                return;
            }

            if (username == Username1)
            {
                _client1Pokemon = offer;
                await Client2.SendMessage($"<TRA offer={offer}>");
            }
            else {
                _client2Pokemon = offer;
                await Client1.SendMessage($"<TRA offer={offer}>");
            }
        }
        private bool _client1Accepted;
        private bool _client2Accepted;
        public async Task Accept(string username)
        {
            if (username == Username1)
            {
                _client1Accepted = true;
                await Client2.SendMessage("<TRA accepted>");
            }
            else {
                _client2Accepted = true;
                await Client1.SendMessage("<TRA accepted>");
            }
            if (_client1Accepted && _client2Accepted)
            {
#pragma warning disable 4014
                TradeLogger.LogTrade(Username1, Username2, _client1Pokemon, _client2Pokemon);
#pragma warning restore 4014
            }
        }
        public async Task Decline(string username)
        {
            if (username == Username1)
            {
                await Client2.SendMessage("<TRA declined>");
            }
            else {
                await Client1.SendMessage("<TRA declined>");
            }
        }
    }
}

