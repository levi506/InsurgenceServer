using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using InsurgenceServerCore.Battles;
using InsurgenceServerCore.Trades;

namespace InsurgenceServerCore.ClientHandler
{
    public class Client
    {
        public readonly TcpClient ActualCient;
        private readonly Socket _socket;

        private Utilities.Version _version;

        public bool Connected = true;
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

            LastActive = DateTime.UtcNow;
            ClientHandler.ActiveClients.TryAdd(Identifier, this);
            _socket = ActualCient.Client;
            var listener = new ListenPipe(this, ActualCient.Client, new Pipe());
            listener.OnCompleteMessage += DataHandler;
            Task.Run(listener.Run).Wait();
        }

        private async Task DataHandler(string data)
        {
            try
            {
                data = data.Replace("\n", "");
                Logger.Logger.Log(Username != "" ? $"{Username} {data}" : $"Not Logged In ({Ip}): {data}");
                LastActive = DateTime.UtcNow;
                var command = new CommandHandler(data);

#pragma warning disable 4014
                NewCommandExecutor.ExecuteCommand(this, command);
#pragma warning restore 4014

            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
        internal async Task ConnectionRequest(string versionStr)
        {
            int result;
            if (!Utilities.Version.TryParse(versionStr, out var version))
            {
                Logger.Logger.Log($"User used invalid version string: {versionStr}");
                return;
            }

            if (version.Major < Data.ServerVersion.Major)
            {
                Logger.Logger.Log($"User used outdated version: {versionStr}");
                result = 0;
            }
            else if (version.Minor < Data.ServerVersion.Minor)
            {
                Logger.Logger.Log($"User used outdated version: {versionStr}");
                result = 0;
            }
            else if (ClientHandler.ActiveClients.Count >= Data.MaximumConnections)
            {
                Logger.Logger.Log($"User was rejected for too many current connections");
                result = 1;
            }
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
                LoggedIn = true;
            }
            if (_version.Major == 6 && _version.Minor == 84 && Admin == false)
            {
                //Ban if user logged in with a debug client, while not being an Admin
#pragma warning disable 4014
                Database.DbUserManagement.Ban(UserId);
                Database.DBWarnLog.LogWarning(UserId, "Automatic ban: Tried to log in with Debug build.");
#pragma warning restore 4014
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
#pragma warning disable 162
                // ReSharper disable once HeuristicUnreachableCode
            {
                // ReSharper disable once HeuristicUnreachableCode
                await SendMessage("<TRA user=nil result=3>");
                // ReSharper disable once HeuristicUnreachableCode
                return;
            }
#pragma warning restore 162
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
            var msg = Encoding.ASCII.GetBytes("<PNG>" + "\n");
            try
            {
                if (_socket.Connected)
                    await _socket.SendAsync(msg, SocketFlags.None);
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
            var msg = Encoding.ASCII.GetBytes(str + "\n");
            try
            {
                if (_socket.Connected)
                    await _socket.SendAsync(msg, SocketFlags.None);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
            var realInput = input.Replace("<", "");
            realInput = realInput.Replace(">", "");
            var arr = realInput.Split('|');
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

