using System;

namespace InsurgenceServer
{
	public class Trade
	{
		public string Username1;
		public string Username2;

		public Client Client1;
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
		public void JoinTrade(Client client)
		{
			Activated = true;
			Client2 = client;
			Client1.SendMessage($"<TRA user={Username2} result=4>");
			Client2.SendMessage($"<TRA user={Username1} result=4>");
		}
		public void ConfirmStart(string username)
		{
			if (username == Username1)
			{
				Client2.SendMessage("<TRA start>");
			}
			else {
				Client1.SendMessage("<TRA start>");
			}
		}
		public void SendParties(string username, string party)
		{
			if (username == Username1)
			{
				Client2.SendMessage($"<TRA party={party}>");
			}
			else {
				Client1.SendMessage($"<TRA party={party}>");
			}
		}
		public void Kill()
		{
            if (Client1 != null && Client1.Connected)
			    Client1.SendMessage("<TRA dead>");
            if (Client2 != null && Client2.Connected)
                Client2?.SendMessage("<TRA dead>");
		    if (Client1 != null)
                Client1.ActiveTrade = null;
		    if (Client2 != null)
                Client2.ActiveTrade = null;
			TradeHandler.DeleteTrade(this);
		}
		private string _client1Pokemon;
		private string _client2Pokemon;
		public void Offer(string username, string offer)
		{
			if (username == Username1)
			{
				_client1Pokemon = offer;
				Client2.SendMessage($"<TRA offer={offer}>");
			}
			else {
				_client2Pokemon = offer;
				Client1.SendMessage($"<TRA offer={offer}>");
			}
		}
		private bool _client1Accepted;
		private bool _client2Accepted;
		public void Accept(string username)
		{
			if (username == Username1)
			{
				_client1Accepted = true;
				Client2.SendMessage("<TRA accepted>");
			}
			else {
				_client2Accepted = true;
				Client1.SendMessage("<TRA accepted>");
			}
			if (_client1Accepted && _client2Accepted)
			{
				TradeLogger.LogTrade(Username1, Username2, _client1Pokemon, _client2Pokemon);
			}
		}
		public void Decline(string username)
		{
			if (username == Username1)
			{
				Client2.SendMessage("<TRA declined>");
			}
			else {
				Client1.SendMessage("<TRA declined>");
			}
		}
	}
}

