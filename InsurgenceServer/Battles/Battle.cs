using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsurgenceServer
{
    public class Battle
    {
        public string username1;
        public string username2;

        public Client Client1;
        public Client Client2;

        public string Trainer1;
        public string Trainer2;

        public bool Activated { get; private set; } = false;
        public DateTime StartTime;

        public int TurnGenerated = -1;
        public int RandSeed;

        public Battle(Client client, string username, string trainer)
        {
            this.username1 = client.Username;
            this.username2 = username;
            this.Client1 = client;
            this.Trainer1 = trainer;
            StartTime = DateTime.UtcNow;
            BattleHandler.ActiveBattles.Add(this);
        }

        public void JoinBattle(Client client, string trainer)
        {
            Activated = true;
            this.Client2 = client;
            this.Trainer2 = trainer;
            Client1.SendMessage(string.Format("<BAT user={0} result=4 trainer={1}>", username2, Trainer2));
            Client2.SendMessage(string.Format("<BAT user={0} result=4 trainer={1}>", username1, Trainer1));
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
            client.SendMessage(string.Format("<BAT seed={0}>", RandSeed));
        }

        public void SendChoice(string username, string choice, string m, string rseed)
        {
            if (username == username1)
            {
                Client2.SendMessage(string.Format("<BAT choices={0} m={1} rseed={2}>", choice, m, rseed));
            }
            else
            {
                Client1.SendMessage(string.Format("<BAT choices={0} m={1} rseed={2}>", choice, m, rseed));
            }
        }

        public void NewPokemon(string username, string New)
        {
            if (username == username1)
            {
                Client2.SendMessage(string.Format("<BAT new={0}>", New));
            }
            else
            {
                Client1.SendMessage(string.Format("<BAT new={0}>", New));
            }
        }

        public void Damage(string username, string damage, string state)
        {
            if (username == username1)
            {
                Client2.SendMessage(string.Format("<BAT damage={0} state={1}>", damage, state));
            }
            else
            {
                Client1.SendMessage(string.Format("<BAT damage={0} state={1}>", damage, state));
            }
        }

        public void Kill()
        {
            Client1.SendMessage("<TRA dead>");
            if (Client2 != null)
                Client2.SendMessage("<TRA dead>");
            Client1.ActiveTrade = null;
            if (Client2 != null)
                Client2.ActiveBattle = null;
            BattleHandler.DeleteBattle(this);
        }
    }
}
