using InsurgenceServer.Battles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsurgenceServer
{
    public static class ExecuteCommand
    {
        public static void Execute(Client client, CommandHandler command)
        {
            if (command.Command == Commands.Null) return;
            else if (command.Command == Commands.DSC)
            {
                client.Disconnect();
            }
            else if (!client._Loggedin)
            {
                if (command.Command == Commands.CON)
                    client.ConnectionRequest(command.data["version"]);
                else if (command.Command == Commands.LOG)
                    client.Login(command.data["user"], command.data["pass"]);
                else if (command.Command == Commands.REG)
                {
                    client.Register(command.data["user"], command.data["pass"], command.data["email"]);
                }
            }
            else if (command.Command == Commands.TRA)
            {
                client.HandleTrade(command.data);
            }
            else if (command.Command == Commands.VBASE)
            {
                var b = Database.DBFriendSafari.GetBase(command.data["user"], client);
                if (b == null) return;
                client.SendMessage(string.Format("<VBASE user={0} result=2 base={1}>", command.data["user"], b));
            }
            else if (command.Command == Commands.UBASE)
            {
                if (Utilities.FSChecker.IsValid(client, command.data["base"]))
                {
                    client.SendMessage("<UBASE result=2>");
                    return;
                }
                Database.DBFriendSafari.UploadBase(client.User_Id, command.data["base"]);
                client.SendMessage("<UBASE result=1>");
            }
            else if (command.Command == Commands.BAT)
            {
                client.HandleBattle(command.data);
            }
            else if (command.Command == Commands.RAND)
            {
                try
                {
                    client.TierSelected = Matchmaking.GetTier(command.data["species"]);
                }
                catch
                {
                    client.TierSelected = Tiers.AG;
                }
                client.SendMessage(string.Format("<RANTIER tier={0}>", Enum.GetName(typeof(Tiers), client.TierSelected)));
            }
            else if (command.Command == Commands.RANBAT)
            {
                if (command.data.ContainsKey("tier"))
                {
                    RandomBattles.AddRandom(client, (Tiers)Enum.Parse(typeof(Tiers),
                        command.data["tier"]), client.TierSelected);
                }
                else if (command.data.ContainsKey("cancel"))
                {
                    RandomBattles.RemoveRandom(client);
                }
                else if (command.data.ContainsKey("decline"))
                {
                    ClientHandler.GetClient(command.data["user"]).SendMessage(string.Format("<RANBAT declined user={0}>", client.Username));
                    RandomBattles.RemoveRandom(client);
                    client.TierSelected = Tiers.Null;
                }
            }
            else if (command.Command == Commands.GTSCREATE)
            {
                GTS.GTSHandler.CreateGTS(client, command.data["offer"], command.data["request"], command.data["index"]);
            }
            else if (command.Command == Commands.GTSOFFER)
            {
                GTS.GTSHandler.OfferGTS(client, command.data["offer"], command.data["id"]);
            }
            else if (command.Command == Commands.GTSREQUEST)
            {
                GTS.GTSHandler.RequestGTS(client, command.data["index"], command.data["filter"]);
            }
            else if (command.Command == Commands.GTSCANCEL)
            {
                GTS.GTSHandler.CancelTrade(client, command.data["id"]);
            }
            else if (command.Command == Commands.GTSCOLLECT)
            {
                GTS.GTSHandler.CollectTrade(client, command.data["id"]);
            }
            else if (command.Command == Commands.GTSMINE)
            {
                GTS.GTSHandler.GetUserTrades(client);
            }
            else if (command.Command == Commands.WTCREATE)
            {
                WonderTrade.WonderTradeHandler.AddTrade(client, command.data["pkmn"]);
            }
            else if (command.Command == Commands.WTCANCEL)
            {
                WonderTrade.WonderTradeHandler.CancelTrade(client);
            }
            else if (command.Command == Commands.DIRGIFT)
            {
                var s = Database.DBMisc.GetDirectGift(client);
                if (s == null)
                    client.SendMessage("<DIRGIFT result=0 gift=nil>");
                else
                    client.SendMessage(string.Format("<DIRGIFT result=1 gift={0}>", s));
            }
        }
    }
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
