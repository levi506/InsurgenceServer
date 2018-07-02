using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using InsurgenceServerCore.Battles;
using InsurgenceServerCore.Database;
using InsurgenceServerCore.Utilities;

namespace InsurgenceServerCore.ClientHandler
{
    public static class NewCommandExecutor
    {
        public static Dictionary<string, MethodInfo> CommandList;
        public static void Initialize()
        {
            CommandList = (typeof(NewCommandExecutor)).GetMethods()
                .Where(y => y.GetCustomAttributes(typeof(ServerCommand), false).Length > 0)
                .ToDictionary(z => z.GetCustomAttribute<ServerCommand>(true).Command);
            Console.WriteLine("The following commands were added:");
            foreach (var kp in CommandList)
            {
                Console.Write(kp.Key + ", ");
            }
            Console.Write("\n");
        }

        public static async Task ExecuteCommand(Client client, CommandHandler command)
        {
            if (!CommandList.TryGetValue(command.Command, out var function))
            {
                Console.WriteLine("Unexpected Command: " + command.Command);
                return;
            }
            if (function.GetCustomAttribute<ServerCommand>(false).RequireLogin && !client.LoggedIn)
                return;
            try
            {
                await (Task)function.Invoke(null, new object[] {client, command});
            }
            catch(Exception e)
            {
                Console.WriteLine($"The following Exception was caught while executing command {command.Command}\n: {e}");
            }
        }

        ///////////////////////////////////////////////////////////
        //Define requests that can be executed by any client here//
        ///////////////////////////////////////////////////////////
        [ServerCommand("DSC", false)]
        public static async Task DisconnectRequest(Client client, CommandHandler command)
        {
            await client.Disconnect();
        }
        [ServerCommand("CON", false)]
        public static async Task ConnectionRequest(Client client, CommandHandler command)
        {
            await client.ConnectionRequest(command.Data["version"]);
        }
        [ServerCommand("LOG", false)]
        public static async Task LoginRequest(Client client, CommandHandler command)
        {
            await client.Login(command.Data["user"], command.Data["pass"]);
        }
        [ServerCommand("REG", false)]
        public static async Task RegisterRequest(Client client, CommandHandler command)
        {
            await client.Register(command.Data["user"], command.Data["pass"], command.Data["email"]);
        }

        [ServerCommand("METADD", false)]
        public static async Task MetricCounterAdd(Client client, CommandHandler command)
        {
            await DbMetrics.MetricCountOne(int.Parse(command.Data["key"]));
        }
        [ServerCommand("METGET", false)]
        public static async Task MetricCounterGet(Client client, CommandHandler command)
        {
            var value = await DbMetrics.GetMetricValue(int.Parse(command.Data["key"]));
            await client.SendMessage($"<METGET val={value}>");
        }

        [ServerCommand("MULTMETADD", false)]
        public static async Task MetricCounterAddMultiple(Client client, CommandHandler command)
        {
            var ls = command.Data["values"].Split(',').ToList().Select(int.Parse).ToList();
            await DbMetrics.MetricCountMultiple(ls);
        }

        ///////////////////////////////////////////////////////////////////////////
        //Define requests that require authentication before execution below here//
        ///////////////////////////////////////////////////////////////////////////
        [ServerCommand("TRA", true)]
        public static async Task TradeRequest(Client client, CommandHandler command)
        {
            await client.HandleTrade(command.Data);
        }
        [ServerCommand("VBASE", true)]
        public static async Task VisitBaseRequest(Client client, CommandHandler command)
        {
            await DbFriendSafari.GetBase(command.Data["user"], client);
        }

        [ServerCommand("VRAND", true)]
        public static async Task VisitRandomBase(Client client, CommandHandler command)
        {
            await DbFriendSafari.GetRandomBase(client);
        }
        [ServerCommand("UBASE", true)]
        public static async Task UploadBaseRequest(Client client, CommandHandler command)
        {
            var b = command.Data["base"].Base64Decode();
            if (!Utilities.FsChecker.IsValid(client, b))
            {
                await client.SendMessage("<UBASE result=0>");
                return;
            }
            await DbFriendSafari.UploadBase(client.UserId, b);
            await client.SendMessage("<UBASE result=1>");
        }

        [ServerCommand("BASEMSG", true)]
        public static async Task ChangeBaseMessageRequest(Client client, CommandHandler command)
        {
            if (string.IsNullOrWhiteSpace(command.Data["message"]) || command.Data["message"] == "nil")
            {
                await DbFriendSafari.RemoveMessage(client.UserId);
            }
            else
            {
                await DbFriendSafari.SetMessage(client.UserId, Utilities.Encoding.Base64Decode(command.Data["message"]));
            }
        }

        [ServerCommand("BASEGIFT", true)]
        public static async Task AddGiftRequest(Client client, CommandHandler command)
        {
            var i = await DbFriendSafari.AddGift(client, uint.Parse(command.Data["gift"]), command.Data["username"]);
            await client.SendMessage($"<BASEGIFT result={i}>");
        }

        [ServerCommand("GETGIFTS", true)]
        public static async Task GetGiftsRequest(Client client, CommandHandler command)
        {
            try
            {
                await DbFriendSafari.GetGifts(client);
            }
            catch
            {
                await client.SendMessage("<FSGIFTS gifts=nil>");
                throw;
            }
        }

        [ServerCommand("BASEBAT", true)]
        public static async Task SetBaseTrainerRequest(Client client, CommandHandler command)
        {
            var trainerString = Utilities.Encoding.Base64Decode(command.Data["trainer"]);
            trainerString = trainerString.Replace("\t", "");
            await DbFriendSafari.SetTrainer(client, trainerString);
        }

        [ServerCommand("GETBAT", false)]
        public static async Task GetBaseTrainerRequest(Client client, CommandHandler command)
        {
            try
            {
                await DbFriendSafari.GetTrainer(client, command.Data["username"]);
            }
            catch
            {
                await client.SendMessage($"<BASETRA result=0 trainer=nil>");
                throw;
            }
        }

        [ServerCommand("BAT", true)]
        public static async Task BattleRequest(Client client, CommandHandler command)
        {
            await client.HandleBattle(command.Data);
        }
        [ServerCommand("RAND", true)]
        public static async Task RandomBattleRequest(Client client, CommandHandler command)
        {
            try
            {
                client.TierSelected = Matchmaking.GetTier(command.Data["species"]);
            }
            catch
            {
                client.TierSelected = Tiers.Ag;
            }
            await client.SendMessage($"<RANTIER tier={Enum.GetName(typeof(Tiers), client.TierSelected)}>");
        }
        [ServerCommand("RANBAT", true)]
        public static async Task TheOtherRandomBattleRequest(Client client, CommandHandler command)
        {
            if (command.Data.ContainsKey("tier"))
            {
                await RandomBattles.AddRandom(client, (Tiers)Enum.Parse(typeof(Tiers),
                    command.Data["tier"], true), client.TierSelected);
            }
            else if (command.Data.ContainsKey("cancel"))
            {
                await RandomBattles.RemoveRandom(client);
            }
            else if (command.Data.ContainsKey("decline"))
            {
                await ClientHandler.GetClient(command.Data["user"]).SendMessage(
                    $"<RANBAT declined user={client.Username}>");
                await RandomBattles.RemoveRandom(client);
                client.TierSelected = Tiers.Null;
            }
        }
        [ServerCommand("GTSCREATE", true)]
        public static async Task GTSCreateRequest(Client client, CommandHandler command)
        {
            try
            {
                await GTS.GtsHandler.CreateGts(client, command.Data["offer"], command.Data["request"], command.Data["index"]);
            }
            catch
            {
                await client.SendMessage($"<GTSCREATE result=2 index={command.Data["index"]}>");
                throw;
            }
        }
        [ServerCommand("GTSREQUEST", true)]
        public static async Task GTSListRequest(Client client, CommandHandler command)
        {
            try
            {
                await GTS.GtsHandler.RequestGts(client, command.Data["index"], command.Data["filter"]);
            }
            catch
            {
                await client.SendMessage("<GTSREQUEST trades=nil>");
                throw;
            }
        }
        [ServerCommand("GTSOFFER", true)]
        public static async Task GTSOfferRequest(Client client, CommandHandler command)
        {
            try
            {
                await GTS.GtsHandler.OfferGts(client, command.Data["offer"], command.Data["id"]);
            }
            catch
            {
                await client.SendMessage("<GTSOFFER result=1 pkmn=nil>");
                throw;
            }
        }
        [ServerCommand("GTSCANCEL", true)]
        public static async Task GTSCancelRequest(Client client, CommandHandler command)
        {
            await GTS.GtsHandler.CancelTrade(client, command.Data["id"]);
        }
        [ServerCommand("GTSCOLLECT", true)]
        public static async Task GTSCollectRequest(Client client, CommandHandler command)
        {
            await GTS.GtsHandler.CollectTrade(client, command.Data["id"]);
        }
        [ServerCommand("GTSMINE", true)]
        public static async Task GTSOwnRequest(Client client, CommandHandler command)
        {
            await GTS.GtsHandler.GetUserTrades(client);
        }
        [ServerCommand("WTCREATE", true)]
        public static async Task CreateWonderTradeRequest(Client client, CommandHandler command)
        {
            await WonderTrade.WonderTradeHandler.AddTrade(client, command.Data["pkmn"]);
        }
        [ServerCommand("WTCANCEL", true)]
        public static async Task CancelWonderTradeRequest(Client client, CommandHandler command)
        {
            await WonderTrade.WonderTradeHandler.CancelTrade(client);
        }
        [ServerCommand("DIRGIFT", true)]
        public static async Task DirectGiftRequest(Client client, CommandHandler command)
        {
            var s = await DbMisc.GetDirectGift(client);
            if (s != null)
            {
                var encode = Utilities.Encoding.Base64Encode(s);
                await client.SendMessage($"<DIRGIFT result=1 gift={encode}>");
            }
            else
            {
                await client.SendMessage($"<DIRGIFT result=0 gift=nil>");
            }
        }
        [ServerCommand("ADDFRIEND", true)]
        public static async Task AddFriendRequest(Client client, CommandHandler command)
        {
            await FriendHandler.AddFriend(client, command.Data["user"]);
        }
        [ServerCommand("REMOVEFRIEND", true)]
        public static async Task RemoveFriendRequest(Client client, CommandHandler command)
        {
            await FriendHandler.RemoveFriend(client, command.Data["user"]);
        }
        [ServerCommand("FRACCEPT", true)]
        public static async Task AcceptFriendRequest(Client client, CommandHandler command)
        {
            await FriendHandler.AcceptRequest(client, command.Data["user"]);
        }
        [ServerCommand("FRDENY", true)]
        public static async Task DenyFriendRequest(Client client, CommandHandler command)
        {
            await FriendHandler.DenyRequest(client, command.Data["user"]);
        }

    }





    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ServerCommand : Attribute
    {
        public string Command { get; }
        public bool RequireLogin { get; }

        public ServerCommand(string command, bool requireLogin)
        {
            Command = command;
            RequireLogin = requireLogin;
        }
    }
}
