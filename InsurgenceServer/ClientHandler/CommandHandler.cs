using InsurgenceServer.Battles;
using System;
using System.Diagnostics.CodeAnalysis;

namespace InsurgenceServer
{
    public static class ExecuteCommand
    {
        public static void Execute(Client client, CommandHandler command)
        {
            if (command.Command == Commands.DSC)
            {
                client.Disconnect();
            }
            else if (!client.Loggedin)
            {
                if (command.Command == Commands.CON)
                    client.ConnectionRequest(command.Data["version"]);
                else if (command.Command == Commands.LOG)
                    client.Login(command.Data["user"], command.Data["pass"]);
                else if (command.Command == Commands.REG)
                {
                    client.Register(command.Data["user"], command.Data["pass"], command.Data["email"]);
                }
            }
            else if (command.Command == Commands.TRA)
            {
                client.HandleTrade(command.Data);
            }
            else if (command.Command == Commands.VBASE)
            {
                var b = Database.DbFriendSafari.GetBase(command.Data["user"], client);
                if (b == null) return;
                client.SendMessage($"<VBASE user={command.Data["user"]} result=2 base={b}>");
            }
            else if (command.Command == Commands.UBASE)
            {
                if (Utilities.FsChecker.IsValid(client, command.Data["base"]))
                {
                    client.SendMessage("<UBASE result=2>");
                    return;
                }
                Database.DbFriendSafari.UploadBase(client.UserId, command.Data["base"]);
                client.SendMessage("<UBASE result=1>");
            }
            else if (command.Command == Commands.BAT)
            {
                client.HandleBattle(command.Data);
            }
            else if (command.Command == Commands.RAND)
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
            else if (command.Command == Commands.RANBAT)
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
            else if (command.Command == Commands.GTSCREATE)
            {
                GTS.GtsHandler.CreateGts(client, command.Data["offer"], command.Data["request"], command.Data["index"]);
            }
            else if (command.Command == Commands.GTSOFFER)
            {
                GTS.GtsHandler.OfferGts(client, command.Data["offer"], command.Data["id"]);
            }
            else if (command.Command == Commands.GTSREQUEST)
            {
                GTS.GtsHandler.RequestGts(client, command.Data["index"], command.Data["filter"]);
            }
            else if (command.Command == Commands.GTSCANCEL)
            {
                GTS.GtsHandler.CancelTrade(client, command.Data["id"]);
            }
            else if (command.Command == Commands.GTSCOLLECT)
            {
                GTS.GtsHandler.CollectTrade(client, command.Data["id"]);
            }
            else if (command.Command == Commands.GTSMINE)
            {
                GTS.GtsHandler.GetUserTrades(client);
            }
            else if (command.Command == Commands.WTCREATE)
            {
                WonderTrade.WonderTradeHandler.AddTrade(client, command.Data["pkmn"]);
            }
            else if (command.Command == Commands.WTCANCEL)
            {
                WonderTrade.WonderTradeHandler.CancelTrade(client);
            }
            else if (command.Command == Commands.DIRGIFT)
            {
                var s = Database.DbMisc.GetDirectGift(client);
                client.SendMessage(s == null ? "<DIRGIFT result=0 gift=nil>" : $"<DIRGIFT result=1 gift={s}>");
            }
            else if (command.Command == Commands.ADDFRIEND)
            {
                FriendHandler.AddFriend(client, command.Data["user"]);
            }
            else if (command.Command == Commands.REMOVEFRIEND)
            {
                FriendHandler.RemoveFriend(client, command.Data["user"]);
            }
            else if (command.Command == Commands.FRACCEPT)
            {
                FriendHandler.AcceptRequest(client, command.Data["user"]);
            }
            else if (command.Command == Commands.FRDENY)
            {
                FriendHandler.DenyRequest(client, command.Data["user"]);
            }
        }
    }
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Commands
    {
        //Default value
        Null = 0,
        //General user functionality
        CON, DSC, LOG, REG,
        //Trading
        TRA,
        //Secret Base functions
        VBASE, UBASE,
        //Battle functions
        BAT, RAND, RANBAT,
        //GTS functions
        GTSCREATE, GTSREQUEST, GTSOFFER, GTSCANCEL, GTSCOLLECT, GTSMINE,
        //Wonder Trade functions
        WTCREATE, WTCANCEL,
        //Direct gift
        DIRGIFT,
        //Friend functions
        ADDFRIEND, REMOVEFRIEND, FRACCEPT, FRDENY
    }
}
