using System;
using System.Threading.Tasks;

namespace InsurgenceServer
{
    public class Battle
    {
        public string Username1;
        public string Username2;

        public Client Client1;
        public Client Client2;

        public string Trainer1;
        public string Trainer2;

        public bool Activated { get; private set; }
        public DateTime StartTime;

        public int Seed;

        public Battle(Client client, string username, string trainer)
        {
            Username1 = client.Username;
            Username2 = username;
            Client1 = client;
            Trainer1 = trainer;
            StartTime = DateTime.UtcNow;
            BattleHandler.ActiveBattles.Add(this);
            Seed = new Random().Next(int.MinValue, int.MaxValue);
        }

        public async Task JoinBattle(Client client, string trainer)
        {
            Activated = true;
            Client2 = client;
            Trainer2 = trainer;
            await Client1.SendMessage($"<BAT user={Username2} result=4 trainer={Trainer2}>");
            await Client2.SendMessage($"<BAT user={Username1} result=4 trainer={Trainer1}>");
        }

        public async Task GetRandomSeed(Client client, string turnString)
        {
            int turn;
            if (!int.TryParse(turnString, out turn))
                return;
            var s =  (Seed << turn | Seed >> 31);
            await client.SendMessage($"<BAT seed={s}>");
        }

        public async Task SendChoice(string username, string choice, string m, string rseed)
        {
            if (username == Username1)
            {
                await Client2.SendMessage($"<BAT choices={choice} m={m} rseed={rseed}>");
            }
            else
            {
                await Client1.SendMessage($"<BAT choices={choice} m={m} rseed={rseed}>");
            }
        }

        public async Task NewPokemon(string username, string New)
        {
            if (username == Username1)
            {
                await Client2.SendMessage($"<BAT new={New}>");
            }
            else
            {
                await Client1.SendMessage($"<BAT new={New}>");
            }
        }

        public async Task Damage(string username, string damage, string state)
        {
            if (username == Username1)
            {
                await Client2.SendMessage($"<BAT damage={damage} state={state}>");
            }
            else
            {
                await Client1.SendMessage($"<BAT damage={damage} state={state}>");
            }
        }

        public async Task Kill()
        {
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
