using System;

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

        public int TurnGenerated = -1;
        public int RandSeed;

        public Battle(Client client, string username, string trainer)
        {
            Username1 = client.Username;
            Username2 = username;
            Client1 = client;
            Trainer1 = trainer;
            StartTime = DateTime.UtcNow;
            BattleHandler.ActiveBattles.Add(this);
        }

        public void JoinBattle(Client client, string trainer)
        {
            Activated = true;
            Client2 = client;
            Trainer2 = trainer;
            Client1.SendMessage($"<BAT user={Username2} result=4 trainer={Trainer2}>");
            Client2.SendMessage($"<BAT user={Username1} result=4 trainer={Trainer1}>");
        }

        public void GetRandomSeed(Client client, string turn)
        {
            int i;
            if (!int.TryParse(turn, out i))
                return;
            if (i > TurnGenerated)
            {
                TurnGenerated = i;
                RandSeed = (DateTime.UtcNow.Millisecond * DateTime.UtcNow.Second);
            }
            client.SendMessage($"<BAT seed={RandSeed}>");
        }

        public void SendChoice(string username, string choice, string m, string rseed)
        {
            if (username == Username1)
            {
                Client2.SendMessage($"<BAT choices={choice} m={m} rseed={rseed}>");
            }
            else
            {
                Client1.SendMessage($"<BAT choices={choice} m={m} rseed={rseed}>");
            }
        }

        public void NewPokemon(string username, string New)
        {
            if (username == Username1)
            {
                Client2.SendMessage($"<BAT new={New}>");
            }
            else
            {
                Client1.SendMessage($"<BAT new={New}>");
            }
        }

        public void Damage(string username, string damage, string state)
        {
            if (username == Username1)
            {
                Client2.SendMessage($"<BAT damage={damage} state={state}>");
            }
            else
            {
                Client1.SendMessage($"<BAT damage={damage} state={state}>");
            }
        }

        public void Kill()
        {
            if (Client1 != null && Client1.Connected)
                Client1?.SendMessage("<TRA dead>");
            if (Client2 != null && Client2.Connected)
                Client2?.SendMessage("<TRA dead>");
            if (Client1 != null) Client1.ActiveBattle = null;
            if (Client2 != null) Client2.ActiveBattle = null;
            BattleHandler.DeleteBattle(this);
        }
    }
}
