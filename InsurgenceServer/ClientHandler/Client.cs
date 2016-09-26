using InsurgenceServer.Battles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace InsurgenceServer
{
	public class Client
	{
	    readonly Byte[] _bytes = new Byte[256];
	    public readonly TcpClient ActualCient;
		private readonly NetworkStream _stream;
        private double _version;

		public bool Connected = true;
		internal bool Loggedin;
        public bool LoggedIn;
		public bool Admin = false;

		public uint UserId;
        public string Username { get; private set; } = "";
		public IPAddress Ip { get; private set; }
		public DateTime LastActive;

		public Trade ActiveTrade;
        public Battle ActiveBattle;

        internal Tiers TierSelected;
        public Tiers QueuedTier;

        public string PendingFriend;
        public List<uint> Friends;

		public Client(TcpClient actualCient)
		{
			ActualCient = actualCient;
			Ip = ((IPEndPoint)actualCient.Client.RemoteEndPoint).Address;
			_stream = ActualCient.GetStream();
			LastActive = DateTime.UtcNow;
			ClientHandler.ActiveClients.Add(this);
		    try
			{
			    int i;
			    while (Connected && (i = _stream.Read(_bytes, 0, _bytes.Length)) != 0)
				{
                    try
                    {
                        var data = System.Text.Encoding.ASCII.GetString(_bytes, 0, i);
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
		private string _message = "";
		public void DataHandler(string data)
		{
			if (data.EndsWith("\n"))
				data = data.Remove(data.Length - 1);
			_message += data;
			if (!data.EndsWith(">"))
			{
				return;
			}

		    Console.WriteLine(Username != "" ? $"{Username} {_message}" : $"Not Logged In: {_message}");

		    LastActive = DateTime.UtcNow;
			var command = new CommandHandler(_message);
			_message = "";

            NewCommandExecutor.ExecuteCommand(this, command);
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
            _version = version;
			SendMessage($"<CON result={result}>");
		}
		internal void Login(string username, string password)
		{
			var result = Database.DbAuthentication.Login(username, password, this);
			//TODO: add ip bans etc here
			if (result == Database.LoginResult.Okay)
			{
				Username = username.ToLower();
				Loggedin = true;
                LoggedIn = true;
			}
            if (Math.Abs(_version - 6.84) < 0.01 && Admin == false)
            {
                //Ban if user logged in with a debug client, while not being an Admin
                Database.DbUserManagement.Ban(UserId);
                SendMessage($"<LOG result={(int) Database.LoginResult.Banned}>");
                return;
            }
			SendMessage($"<LOG result={(int) result}>");
            Friends = Database.DbFriendHandler.GetFriends(this);

            //We encode each name into base64 to prevent commas in names from breaking stuff
            var compilefriends = string.Join(",", Friends.Select(x => Utilities.Encoding.Base64Encode(x.ToString())));
            var onlinestring = Friends.Aggregate("", (current, friend) => current + (((ClientHandler.GetClient(friend)) == null) ? "0" : "1"));

		    SendMessage($"<FRIENDLIST friends={compilefriends} online={onlinestring}>");

            System.Threading.Tasks.Task.Run(() => FriendHandler.NotifyFriends(this, true));
		}
        internal void Register(string username, string password, string email)
        {
            Database.DbAuthentication.Register(this, username, password, email);
        }
        internal void HandleTrade(Dictionary<string, string> args)
		{
			if (args.ContainsKey("user"))
			{
				var t = TradeHandler.BeginTrade(args["user"], this);
				if (t != null)
					ActiveTrade = t;
			}
			else if (args.ContainsKey("start"))
			{
			    ActiveTrade?.ConfirmStart(Username);
			}
			else if (args.ContainsKey("party"))
			{
			    ActiveTrade?.SendParties(Username, args["party"]);
			}
			else if (args.ContainsKey("dead"))
			{
			    ActiveTrade?.Kill();
			}
			else if (args.ContainsKey("offer"))
			{
			    ActiveTrade?.Offer(Username, args["offer"]);
			}
			else if (args.ContainsKey("accepted"))
			{
			    ActiveTrade?.Accept(Username);
			}
			else if (args.ContainsKey("declined"))
			{
			    ActiveTrade?.Decline(Username);
			}
		}

        internal void HandleBattle(Dictionary<string, string> args)
        {
            if (args.ContainsKey("user"))
            {
                var b = BattleHandler.BeginBattle(args["user"], this, args["trainer"]);
                if (b != null)
                    ActiveBattle = b;
            }
            else if (args.ContainsKey("seed"))
            {
                ActiveBattle.GetRandomSeed(this, args["turn"]);
            }
            else if (args.ContainsKey("choices"))
            {
                ActiveBattle.SendChoice(Username, args["choices"], args["m"], args["rseed"]);
            }
            else if (args.ContainsKey("new"))
            {
                ActiveBattle.NewPokemon(Username, args["new"]);
            }
            else if (args.ContainsKey("damage"))
            {
                ActiveBattle.Damage(Username, args["damage"], args["state"]);
            }
        }

        public void Ping()
        {
            byte[] msg = System.Text.Encoding.ASCII.GetBytes("<PNG>" + "\n");
            try
            {
                _stream.Write(msg, 0, msg.Length);
                _stream.Flush();
            }
            catch {
                Disconnect();
            }
        }

		public void SendMessage(string str)
		{
			if (!ActualCient.Connected)
			{
				Disconnect();
				return;
			}
			byte[] msg = System.Text.Encoding.ASCII.GetBytes(str + "\n");
			try
			{
				_stream.Write(msg, 0, msg.Length);
				_stream.Flush();
			}
		    catch
		    {
		        // ignored
		    }
		}
		public void Disconnect()
		{
            System.Threading.Tasks.Task.Run(() => FriendHandler.NotifyFriends(this, false));
            Connected = false;
			try
			{
                ClientHandler.Remove(this);
			    ActiveTrade?.Kill();
			    ActiveBattle?.Kill();
			    if (QueuedTier != Tiers.Null)
                    RandomBattles.RemoveRandom(this);
                WonderTrade.WonderTradeHandler.DeleteFromClient(this);
                SendMessage("<DSC>");
				_stream.Close();
				ActualCient.Close();
			}
			catch (Exception e) {
                Logger.ErrorLog.Log(e);
				Debug.WriteLine(e);
			}
		}
	}
	public class CommandHandler
	{
		public string Command;
		public Dictionary<string, string> Data = new Dictionary<string, string>();
		public CommandHandler(string input)
		{
		    if (input == null) throw new ArgumentNullException(nameof(input));
		    if (!(input.StartsWith("<") && input.EndsWith(">")))
				return;
			var realInput = input.Remove(0, 1);
			realInput = realInput.Remove(realInput.Length - 1);
			var arr = realInput.Split('\t');
		    Command = arr[0];
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
				Data.Add(carr[0], arg);
			}
		}
	}
	
}

