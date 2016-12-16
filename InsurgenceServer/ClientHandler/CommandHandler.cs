using InsurgenceServer.Battles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InsurgenceServer
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

        public static void ExecuteCommand(Client client, CommandHandler command)
        {
            MethodInfo function;
            if (!CommandList.TryGetValue(command.Command, out function))
            {
                Console.WriteLine("Unexpected Command: " + command.Command);
                return;
            }
            if (function.GetCustomAttribute<ServerCommand>(false).RequireLogin && !client.LoggedIn)
                return;
            try
            {
                function.Invoke(null, new object[] {client, command});
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
        public static void DisconnectRequest(Client client, CommandHandler command)
        {
            client.Disconnect();
        }
        [ServerCommand("CON", false)]
        public static void ConnectionRequest(Client client, CommandHandler command)
        {
            client.ConnectionRequest(command.Data["version"]);
        }
        [ServerCommand("LOG", false)]
        public static void LoginRequest(Client client, CommandHandler command)
        {
            client.Login(command.Data["user"], command.Data["pass"]);
        }
        [ServerCommand("REG", false)]
        public static void RegisterRequest(Client client, CommandHandler command)
        {
            client.Register(command.Data["user"], command.Data["pass"], command.Data["email"]);
        }

        [ServerCommand("METADD", false)]
        public static void MetricCounterAdd(Client client, CommandHandler command)
        {
            Database.DbMetrics.MetricCountOne(int.Parse(command.Data["key"]));
        }
        [ServerCommand("METGET", false)]
        public static void MetricCounterGet(Client client, CommandHandler command)
        {
            var value = Database.DbMetrics.GetMetricValue(int.Parse(command.Data["key"]));
            client.SendMessage($"<METGET val={value}>");
        }

        [ServerCommand("MULTMETADD", false)]
        public static void MetricCounterAddMultiple(Client client, CommandHandler command)
        {
            var ls = command.Data["values"].Split(',').ToList().Select(int.Parse).ToList();
            Database.DbMetrics.MetricCountMultiple(ls);
        }

        ///////////////////////////////////////////////////////////////////////////
        //Define requests that require authentication before execution below here//
        ///////////////////////////////////////////////////////////////////////////
        [ServerCommand("TRA", true)]
        public static void TradeRequest(Client client, CommandHandler command)
        {
            client.HandleTrade(command.Data);
        }
        [ServerCommand("VBASE", true)]
        public static void VisitBaseRequest(Client client, CommandHandler command)
        {
            Database.DbFriendSafari.GetBase(command.Data["user"], client);
        }
        [ServerCommand("UBASE", true)]
        public static void UploadBaseRequest(Client client, CommandHandler command)
        {
            if (Utilities.FsChecker.IsValid(client, command.Data["base"]))
            {
                client.SendMessage("<UBASE result=2>");
                return;
            }
            Database.DbFriendSafari.UploadBase(client.UserId, command.Data["base"]);
            client.SendMessage("<UBASE result=1>");
        }

        [ServerCommand("BASEMSG", true)]
        public static void ChangeBaseMessageRequest(Client client, CommandHandler command)
        {
            if (string.IsNullOrWhiteSpace(command.Data["message"]))
            {
                Database.DbFriendSafari.RemoveMessage(client.UserId);
            }
            else
            {
                Database.DbFriendSafari.SetMessage(client.UserId, command.Data["message"]);
            }
        }

        [ServerCommand("BASEGIFT", true)]
        public static void AddGiftRequest(Client client, CommandHandler command)
        {
            var i = Database.DbFriendSafari.AddGift(client, uint.Parse(command.Data["gift"]), command.Data["username"]);
            client.SendMessage($"<BASEGIFT result={i}>");
        }

        [ServerCommand("GETGIFTS", true)]
        public static void GetGiftsRequest(Client client, CommandHandler command)
        {
            Database.DbFriendSafari.GetGifts(client);
        }


        [ServerCommand("BAT", true)]
        public static void BattleRequest(Client client, CommandHandler command)
        {
            client.HandleBattle(command.Data);
        }
        [ServerCommand("RAND", true)]
        public static void RandomBattleRequest(Client client, CommandHandler command)
        {
            try
            {
                client.TierSelected = Matchmaking.GetTier(command.Data["species"]);
            }
            catch
            {
                client.TierSelected = Tiers.Ag;
            }
            client.SendMessage($"<RANTIER tier={Enum.GetName(typeof(Tiers), client.TierSelected)}>");
        }
        [ServerCommand("RANBAT", true)]
        public static void TheOtherRandomBattleRequest(Client client, CommandHandler command)
        {
            if (command.Data.ContainsKey("tier"))
            {
                RandomBattles.AddRandom(client, (Tiers)Enum.Parse(typeof(Tiers),
                    command.Data["tier"]), client.TierSelected);
            }
            else if (command.Data.ContainsKey("cancel"))
            {
                RandomBattles.RemoveRandom(client);
            }
            else if (command.Data.ContainsKey("decline"))
            {
                ClientHandler.GetClient(command.Data["user"]).SendMessage(
                    $"<RANBAT declined user={client.Username}>");
                RandomBattles.RemoveRandom(client);
                client.TierSelected = Tiers.Null;
            }
        }
        [ServerCommand("GTSCREATE", true)]
        public static void GTSCreateRequest(Client client, CommandHandler command)
        {
            GTS.GtsHandler.CreateGts(client, command.Data["offer"], command.Data["request"], command.Data["index"]);
        }
        [ServerCommand("GTSREQUEST", true)]
        public static void GTSListRequest(Client client, CommandHandler command)
        {
            GTS.GtsHandler.RequestGts(client, command.Data["index"], command.Data["filter"]);
        }
        [ServerCommand("GTSOFFER", true)]
        public static void GTSOfferRequest(Client client, CommandHandler command)
        {
            GTS.GtsHandler.OfferGts(client, command.Data["offer"], command.Data["id"]);
        }
        [ServerCommand("GTSCANCEL", true)]
        public static void GTSCancelRequest(Client client, CommandHandler command)
        {
            GTS.GtsHandler.CancelTrade(client, command.Data["id"]);
        }
        [ServerCommand("GTSCOLLECT", true)]
        public static void GTSCollectRequest(Client client, CommandHandler command)
        {
            GTS.GtsHandler.CollectTrade(client, command.Data["id"]);
        }
        [ServerCommand("GTSMINE", true)]
        public static void GTSOwnRequest(Client client, CommandHandler command)
        {
            GTS.GtsHandler.GetUserTrades(client);
        }
        [ServerCommand("WTCREATE", true)]
        public static void CreateWonderTradeRequest(Client client, CommandHandler command)
        {
            WonderTrade.WonderTradeHandler.AddTrade(client, command.Data["pkmn"]);
        }
        [ServerCommand("WTCANCEL", true)]
        public static void CancelWonderTradeRequest(Client client, CommandHandler command)
        {
            WonderTrade.WonderTradeHandler.CancelTrade(client);
        }
        [ServerCommand("DIRGIFT", true)]
        public static void DirectGiftRequest(Client client, CommandHandler command)
        {
            var s = Database.DbMisc.GetDirectGift(client);
            client.SendMessage(s == null ? "<DIRGIFT result=0 gift=nil>" : $"<DIRGIFT result=1 gift={s}>");
        }
        [ServerCommand("ADDFRIEND", true)]
        public static void AddFriendRequest(Client client, CommandHandler command)
        {
            FriendHandler.AddFriend(client, command.Data["user"]);
        }
        [ServerCommand("REMOVEFRIEND", true)]
        public static void RemoveFriendRequest(Client client, CommandHandler command)
        {
            FriendHandler.RemoveFriend(client, command.Data["user"]);
        }
        [ServerCommand("FRACCEPT", true)]
        public static void AcceptFriendRequest(Client client, CommandHandler command)
        {
            FriendHandler.AcceptRequest(client, command.Data["user"]);
        }
        [ServerCommand("FRDENY", true)]
        public static void DenyFriendRequest(Client client, CommandHandler command)
        {
            FriendHandler.DenyRequest(client, command.Data["user"]);
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
