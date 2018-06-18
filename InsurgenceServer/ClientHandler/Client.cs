using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using InsurgenceServer.Battles;
using InsurgenceServer.Trades;

namespace InsurgenceServer.ClientHandler
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

        public Guid Identifier;
        public uint UserId;
        public string Username { get; private set; } = "";
        public IPAddress Ip { get; private set; }
        public DateTime LastActive;

        public Guid ActiveBattleId;
        public Trade ActiveTrade;
        public Battle ActiveBattle;

        internal Tiers TierSelected;
        public Tiers QueuedTier;

        public string PendingFriend;
        public List<uint> Friends;

        public bool IsConnected => ActualCient.Connected;

        public Client(TcpClient actualCient)
        {
            Identifier = Guid.NewGuid();
            ActualCient = actualCient;
            Ip = ((IPEndPoint)actualCient.Client.RemoteEndPoint).Address;
            _stream = ActualCient.GetStream();
            LastActive = DateTime.UtcNow;
            ClientHandler.ActiveClients.TryAdd(Identifier, this);
#pragma warning disable 4014
            StartListening();
#pragma warning restore 4014
        }
        private string _message = "";

        public async Task StartListening()
        {
            try
            {
                int i;
                while (Connected && (i = _stream.Read(_bytes, 0, _bytes.Length)) != 0)
                {
                    try
                    {
                        var data = System.Text.Encoding.ASCII.GetString(_bytes, 0, i);
                        await DataHandler(data);
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

        public async Task DataHandler(string data)
        {
            try
            {
                if (data.EndsWith("\n"))
                    data = data.Remove(data.Length - 1);
                _message += data;
                if (!data.EndsWith(">"))
                {
                    return;
                }

                Logger.Logger.Log(Username != "" ? $"{Username} {_message}" : $"Not Logged In ({Ip}): {_message}");
                if (_message.EndsWith("<DSC>") && _message.Length > 5)
                {
                    LastActive = DateTime.UtcNow;
                    var msg = _message.Replace("<DSC>", "");
                    var command = new CommandHandler(msg);
#pragma warning disable 4014
                    NewCommandExecutor.ExecuteCommand(this, command);
#pragma warning restore 4014

                    var disconnectCommand = new CommandHandler("<DSC>");
#pragma warning disable 4014
                    NewCommandExecutor.ExecuteCommand(this, disconnectCommand);
#pragma warning restore 4014
                }
                else
                {
                    LastActive = DateTime.UtcNow;
                    var command = new CommandHandler(_message);
                    _message = "";

#pragma warning disable 4014
                    NewCommandExecutor.ExecuteCommand(this, command);
#pragma warning restore 4014
                }

            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
        internal async Task ConnectionRequest(string versionstr)
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
            await SendMessage($"<CON result={result}>");
        }
        internal async Task Login(string username, string password)
        {
            var result = await Database.DbAuthentication.Login(username, password, this);
            if (result == Database.LoginResult.Okay)
            {
                Username = username.ToLower();
                Loggedin = true;
                LoggedIn = true;
            }
            if (Math.Abs(_version - 6.84) < 0.01 && Admin == false)
            {
                //Ban if user logged in with a debug client, while not being an Admin
                await Database.DbUserManagement.Ban(UserId);
                await SendMessage($"<LOG result={(int) Database.LoginResult.Banned}>");
                return;
            }
            await SendMessage($"<LOG result={(int) result}>");
            Friends = await Database.DbFriendHandler.GetFriends(this);

            //We encode each name into base64 to prevent commas in names from breaking stuff
            var compilefriends = string.Join(",", Friends.Select(x => Utilities.Encoding.Base64Encode(x.ToString())));
            var onlinestring = Friends.Aggregate("",
                (current, friend) => current + (((ClientHandler.GetClient(friend)) == null) ? "0" : "1"));

            await SendMessage($"<FRIENDLIST friends={compilefriends} online={onlinestring}>");

#pragma warning disable 4014
            FriendHandler.NotifyFriends(this, true);
#pragma warning restore 4014
        }

        internal async Task Register(string username, string password, string email)
        {
            await Database.DbAuthentication.Register(this, username, password, email);
        }
        internal async Task HandleTrade(Dictionary<string, string> args)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (Data.TradeDisabled)
            {
                await SendMessage("<TRA user=nil result=3>");
                return;
            }
            if (args.ContainsKey("user"))
            {
                var t = await TradeHandler.BeginTrade(args["user"], this);
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
                ActiveTrade?.Offer(this, Username, args["offer"]);
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

        internal async Task HandleBattle(Dictionary<string, string> args)
        {
            if (args.ContainsKey("user"))
            {
                var b = await BattleHandler.BeginBattle(args["user"], this, args["trainer"]);
                if (b == null)
                    return;
                ActiveBattle = b;
                ActiveBattleId = b.Id;
                return;
            }
            if (ActiveBattle == null)
            {
                ActiveBattle = BattleHandler.GetBattle(this);
            }
            if (ActiveBattle == null)
            {
                Logger.Logger.Log($"Can't find battle for user: {Username}");
                await SendMessage("<TRA dead>");
                return;
            }
            if (args.ContainsKey("seed"))
            {
                await ActiveBattle.GetRandomSeed(this, args["turn"]);
            }
            else if (args.ContainsKey("choices"))
            {
                await ActiveBattle.SendChoice(Username, args["choices"], args["m"], args["rseed"]);
            }
            else if (args.ContainsKey("new"))
            {
                await ActiveBattle.NewPokemon(Username, args["new"]);
            }
            else if (args.ContainsKey("damage"))
            {
                await ActiveBattle.Damage(Username, args["damage"], args["state"]);
            }
        }

        public async Task Ping()
        {
            var msg = System.Text.Encoding.ASCII.GetBytes("<PNG>" + "\n");
            try
            {
                await _stream.WriteAsync(msg, 0, msg.Length);
                await _stream.FlushAsync();
            }
            catch {
                await Disconnect();
            }
        }

        public async Task SendMessage(string str, bool ignoreDisconnecting = false)
        {
            if (!ActualCient.Connected && !ignoreDisconnecting)
            {
                await Disconnect();
                return;
            }
            var msg = System.Text.Encoding.ASCII.GetBytes(str + "\n");
            try
            {
                await _stream.WriteAsync(msg, 0, msg.Length);
                await _stream.FlushAsync();
            }
            catch
            {
                // ignored
            }
        }
        public async Task Disconnect()
        {
#pragma warning disable 4014
            FriendHandler.NotifyFriends(this, false);
#pragma warning restore 4014
            Connected = false;
            try
            {
                await ClientHandler.Remove(this);
                ActiveTrade?.Kill();
                ActiveBattle?.Kill($"{Username} disconnected");
                if (QueuedTier != Tiers.Null)
                    await RandomBattles.RemoveRandom(this);
                await WonderTrade.WonderTradeHandler.DeleteFromClient(this);
                await SendMessage("<DSC>", true);
                _stream.Close();
                ActualCient.Close();
            }
            catch (Exception e) {
                Logger.Logger.Log(e.ToString());
            }
        }
    }
    public class CommandHandler
    {
        public readonly string Command;
        public readonly Dictionary<string, string> Data = new Dictionary<string, string>();
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

