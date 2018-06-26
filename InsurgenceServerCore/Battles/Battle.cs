using System;
using System.Threading.Tasks;
using InsurgenceServerCore.ClientHandler;

namespace InsurgenceServerCore.Battles
{
    public class Battle
    {
        public Guid Id;

        public string Username1;
        public string Username2;

        public Client Client1;
        public Client Client2;

        public string Trainer1;
        public string Trainer2;

        public bool Activated { get; private set; }
        public DateTime StartTime;
        public DateTime LastMessageTime;

        public int Seed;

        public Battle(Client client, string username, string trainer)
        {
            Id = Guid.NewGuid();
            Username1 = client.Username.ToLowerInvariant();
            Username2 = username.ToLowerInvariant();
            Client1 = client;
            Trainer1 = trainer;
            StartTime = DateTime.UtcNow;
            LastMessageTime = DateTime.UtcNow;
            BattleHandler.ActiveBattles.TryAdd(Id, this);
            Seed = new Random().Next(int.MinValue, int.MaxValue);
        }

        public async Task JoinBattle(Client client, string trainer)
        {
            Activated = true;
            Client2 = client;
            Trainer2 = trainer;
            LastMessageTime = DateTime.UtcNow;
            await SendMessage(1, $"<BAT user={Username2} result=4 trainer={Trainer2}>");
            await SendMessage(2, $"<BAT user={Username1} result=4 trainer={Trainer1}>");
        }

        public async Task SendMessage(int clientId, string message)
        {
            if (clientId == 1)
            {
                if (!Client1.IsConnected)
                {
                    await Client2.SendMessage("<TRA dead>");
                    return;
                }
                await Client1.SendMessage(message);
            }
            else
            {
                if (!Client2.IsConnected)
                {
                    await Client1.SendMessage("<TRA dead>");
                    return;
                }
                await Client2.SendMessage(message);
            }
        }

        public async Task GetRandomSeed(Client client, string turnString)
        {
            LastMessageTime = DateTime.UtcNow;
            int turn;
            if (!int.TryParse(turnString, out turn))
                return;
            var s =  (Seed << turn | Seed >> 31);
            if (client.UserId == Client1.UserId)
                await SendMessage(1, $"<BAT seed={s}>");
            else
                await SendMessage(2, $"<BAT seed={s}>");
        }

        public async Task SendChoice(string username, string choice, string m, string rseed)
        {
            LastMessageTime = DateTime.UtcNow;
            if (username == Username1)
            {
                await SendMessage(2, $"<BAT choices={choice} m={m} rseed={rseed}>");
            }
            else
            {
                await SendMessage(1, $"<BAT choices={choice} m={m} rseed={rseed}>");
            }
        }

        public async Task NewPokemon(string username, string New)
        {
            LastMessageTime = DateTime.UtcNow;
            if (username == Username1)
            {
                await SendMessage(2, $"<BAT new={New}>");
            }
            else
            {
                await SendMessage(1, $"<BAT new={New}>");
            }
        }

        public async Task Damage(string username, string damage, string state)
        {
            LastMessageTime = DateTime.UtcNow;
            if (username == Username1)
            {
                await SendMessage(2, $"<BAT damage={damage} state={state}>");
            }
            else
            {
                await SendMessage(1, $"<BAT damage={damage} state={state}>");
            }
        }

        public async Task Kill(string reason)
        {
            Logger.Logger.Log($"Killing battle between {Username1} and {Username2} because of {reason}");
            if (Client1 != null && Client1.Connected)
                await Client1.SendMessage("<TRA dead>");
            if (Client2 != null && Client2.Connected)
                await Client2.SendMessage("<TRA dead>");
            if (Client1 != null) Client1.ActiveBattle = null;
            if (Client2 != null) Client2.ActiveBattle = null;
            BattleHandler.DeleteBattle(this);
        }
    }
}
