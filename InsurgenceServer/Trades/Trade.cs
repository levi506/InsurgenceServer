using System;
using System.Threading;
using System.Threading.Tasks;

namespace InsurgenceServer
{
	public class Trade
	{
		public string username1;
		public string username2;

		public Client Client1;
		public Client Client2;

		public bool Activated { get; private set; } = false;
		public DateTime StartTime;

		public Trade(Client client, string username)
		{
			this.username1 = client.Username;
			this.username2 = username;
			this.Client1 = client;
			StartTime = DateTime.UtcNow;
			TradeHandler.ActiveTrades.Add(this);

		}
		public void JoinTrade(Client client)
		{
			Activated = true;
			this.Client2 = client;
			Client1.SendMessage(string.Format("<TRA user={0} result=4>", username2));
			Client2.SendMessage(string.Format("<TRA user={0} result=4>", username1));
		}
		public void ConfirmStart(string username)
		{
			if (username == this.username1)
			{
				Client2.SendMessage("<TRA start>");
			}
			else {
				Client1.SendMessage("<TRA start>");
			}
		}
		public void SendParties(string username, string party)
		{
			if (username == this.username1)
			{
				Client2.SendMessage(string.Format("<TRA party={0}>", party));
			}
			else {
				Client1.SendMessage(string.Format("<TRA party={0}>", party));
			}
		}
		public void Kill()
		{
			Client1.SendMessage("<TRA dead>");
			if (Client2 != null)
				Client2.SendMessage("<TRA dead>");
			Client1.ActiveTrade = null;
			if (Client2 != null)
				Client2.ActiveTrade = null;
			TradeHandler.DeleteTrade(this);
		}
		private string Client1Pokemon;
		private string Client2Pokemon;
		public void Offer(string username, string offer)
		{
			if (username == this.username1)
			{
				Client1Pokemon = offer;
				Client2.SendMessage(string.Format("<TRA offer={0}>", offer));
			}
			else {
				Client2Pokemon = offer;
				Client1.SendMessage(string.Format("<TRA offer={0}>", offer));
			}
		}
		private bool Client1Accepted;
		private bool Client2Accepted;
		public void Accept(string username)
		{
			if (username == this.username1)
			{
				Client1Accepted = true;
				Client2.SendMessage("<TRA accepted>");
			}
			else {
				Client2Accepted = true;
				Client1.SendMessage("<TRA accepted>");
			}
			if (Client1Accepted && Client2Accepted)
			{
				TradeLogger.LogTrade(username1, username2, Client1Pokemon, Client2Pokemon);
			}
		}
		public void Decline(string username)
		{
			if (username == this.username1)
			{
				Client2.SendMessage("<TRA declined>");
			}
			else {
				Client1.SendMessage("<TRA declined>");
			}
		}
	}
}

