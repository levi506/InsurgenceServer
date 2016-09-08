using InsurgenceServer.Battles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace InsurgenceServer
{
	public class Client
	{
		Byte[] bytes = new Byte[256];
		public TcpClient _client;
		private NetworkStream stream;
        private double Version;

		public bool Connected = true;
		internal bool _Loggedin;
        public bool LoggedIn;
		public bool Admin = false;

		public uint User_Id;
        public string Username { get; private set; } = "";
		public IPAddress IP { get; private set; }
		public DateTime LastActive;

		public Trade ActiveTrade;
        public Battle ActiveBattle;

        internal Tiers TierSelected;
        public Tiers QueuedTier;

		public Client(TcpClient client)
		{
			this._client = client;
			this.IP = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
			stream = _client.GetStream();
			LastActive = DateTime.UtcNow;
			ClientHandler.ActiveClients.Add(this);
			int i;
			try
			{
				while (Connected && (i = stream.Read(bytes, 0, bytes.Length)) != 0)
				{
                    try
                    {
                        var data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        DataHandler(data);
                    }
                    catch (Exception e) {
                        Console.WriteLine(e);
                    }
                }
			}
			catch (System.IO.IOException)
			{
			}
			catch (Exception e)
			{
                Logger.ErrorLog.Log(e);
                Console.WriteLine(e);
			}
		}
		private string Message = "";
		public void DataHandler(string data)
		{
			if (data.EndsWith("\n"))
				data = data.Remove(data.Length - 1);
			Message += data;
			if (!data.EndsWith(">"))
			{
				return;
			}
            
            if (Username != "")
                Console.WriteLine(string.Format("{0} {1}", Username, Message));
            else
                Console.WriteLine(string.Format("Not Logged In: {0}", Message));
            
			LastActive = DateTime.UtcNow;
			var command = new CommandHandler(Message);
			Message = "";

            ExecuteCommand.Execute(this, command);
        }
		internal void ConnectionRequest(string versionstr)
		{
			double version;
			int result;
			if (!double.TryParse(versionstr, out version))
				return;
			if (version < Data.ServerVersion)
				result = 0;
			else if (ClientHandler.ActiveClients.Count >= Data.MaximumConnections)
				result = 1;
			else
				result = 2;
            this.Version = version;
			SendMessage(string.Format("<CON result={0}>", result));
		}
		internal void Login(string username, string password)
		{
			var result = Database.DBAuthentication.Login(username, password, this);
			//TODO: add ip bans etc here
			if (result == Database.LoginResult.Okay)
			{
				this.Username = username.ToLower();
				this._Loggedin = true;
                this.LoggedIn = true;
			}
            if (this.Version == 6.84 && this.Admin == false)
            {
                //Ban if user logged in with a debug client, while not being an Admin
                Database.DBUserManagement.Ban(this.User_Id);
                SendMessage(string.Format("<LOG result={0}>", (int)Database.LoginResult.Banned));
                return;
            }
			SendMessage(string.Format("<LOG result={0}>", (int)result));
		}
        internal void Register(string username, string password, string email)
        {
            Database.DBAuthentication.Register(this, username, password, email);
        }
        internal void HandleTrade(Dictionary<string, string> args)
		{
			if (args.ContainsKey("user"))
			{
				var t = TradeHandler.BeginTrade(args["user"], this);
				if (t != null)
					this.ActiveTrade = t;
			}
			else if (args.ContainsKey("start"))
			{
				if (this.ActiveTrade != null)
					this.ActiveTrade.ConfirmStart(this.Username);
			}
			else if (args.ContainsKey("party"))
			{
				if (this.ActiveTrade != null)
					this.ActiveTrade.SendParties(this.Username, args["party"]);
			}
			else if (args.ContainsKey("dead"))
			{
				if (this.ActiveTrade != null)
					this.ActiveTrade.Kill();
			}
			else if (args.ContainsKey("offer"))
			{
				if (this.ActiveTrade != null)
					this.ActiveTrade.Offer(this.Username, args["offer"]);
			}
			else if (args.ContainsKey("accepted"))
			{
				if (this.ActiveTrade != null)
					this.ActiveTrade.Accept(this.Username);
			}
			else if (args.ContainsKey("declined"))
			{
				if (this.ActiveTrade != null)
					this.ActiveTrade.Decline(this.Username);
			}
		}

        internal void HandleBattle(Dictionary<string, string> args)
        {
            if (args.ContainsKey("user"))
            {
                var b = BattleHandler.BeginBattle(args["user"], this, args["trainer"]);
                if (b != null)
                    this.ActiveBattle = b;
            }
            else if (args.ContainsKey("seed"))
            {
                ActiveBattle.GetRandomSeed(this, args["turn"]);
            }
            else if (args.ContainsKey("choices"))
            {
                ActiveBattle.SendChoice(this.Username, args["choices"], args["m"], args["rseed"]);
            }
            else if (args.ContainsKey("new"))
            {
                ActiveBattle.NewPokemon(this.Username, args["new"]);
            }
            else if (args.ContainsKey("damage"))
            {
                ActiveBattle.Damage(this.Username, args["damage"], args["state"]);
            }
        }

		public void SendMessage(string str)
		{
			if (!_client.Connected)
			{
				Disconnect();
				return;
			}
			byte[] msg = System.Text.Encoding.ASCII.GetBytes(str + "\n");
			try
			{
				stream.Write(msg, 0, msg.Length);
				stream.Flush();
			}
			catch { }
		}
		public void Disconnect()
		{
			Connected = false;
			try
			{
                ClientHandler.Remove(this);
                if (ActiveTrade != null)
					ActiveTrade.Kill();
                if (ActiveBattle != null)
                    ActiveBattle.Kill();
                if (QueuedTier != Tiers.Null)
                    RandomBattles.RemoveRandom(this);
                WonderTrade.WonderTradeHandler.DeleteFromClient(this);
                SendMessage("<DSC>");
				stream.Close();
				_client.Close();
			}
			catch (Exception e) {
                Logger.ErrorLog.Log(e);
				Debug.WriteLine(e);
			}
		}
	}
	public class CommandHandler
	{
		public Commands Command;
		public Dictionary<string, string> data = new Dictionary<string, string>();
		public CommandHandler(string _input)
		{
			if (!(_input.StartsWith("<") && _input.EndsWith(">")))
				return;
			var input = _input.Remove(0, 1);
			input = input.Remove(input.Length - 1);
			var arr = input.Split('\t');
			if (!Enum.TryParse<Commands>(arr[0], out Command))
			{
				Console.WriteLine("Unexpected Command: " + arr[0]);
				return;
			}
			for (var i = 1; i < arr.Length; i++)
			{
				if (arr[i] == "")
					continue;
				var carr = arr[i].Split('=');
				var arg = "";
				for (var j = 1; j < carr.Length; j++)
				{
					arg += carr[j];
					if (j != 1)
						arg += "=";
				}
				data.Add(carr[0], arg);
			}
		}
	}
	public enum Commands
	{
		Null = 0, CON, DSC, LOG, REG, TRA, VBASE, UBASE, BAT, RAND, RANBAT, GTSCREATE, GTSREQUEST, GTSOFFER,
        GTSCANCEL, GTSCOLLECT, GTSMINE, WTCREATE, WTCANCEL 
	}
}

