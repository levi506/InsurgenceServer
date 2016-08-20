using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsurgenceServer.GTS
{
    public static class GTSHandler
    {
        public static void CreateGTS(Client c,string Offer, string Request, string index)
        {
            //If user is banned, return
            if (Database.DBUserChecks.UserBanned(c.Username))
            {
                c.SendMessage(string.Format("<GTSCREATE result=0 index={0}>", index));
                return;
            }
            //If user has more than max amount of allowed GTS trades, return
            if (Database.DBGTS.GetNumberOfTrades(c.User_Id) >= Data.MaximumGTSTradesPerUser)
            {
                c.SendMessage(string.Format("<GTSCREATE result=1 index={0}>", index));
                return;
            }
            //Input in database
            try
            {
                Database.DBGTS.Add(c.User_Id, Offer, Request);
                c.SendMessage(string.Format("<GTSCREATE result=3 index={0}>", index));
            }
            catch
            {
                c.SendMessage(string.Format("<GTSCREATE result=2 index={0}>", index));
            }
        }
        public static void RequestGTS(Client c, string lastIDstr)
        {
            uint index;
            if (!uint.TryParse(lastIDstr, out index))
                return;
            //Request pokemon from the database, starting with pokemon last seen + 1. If lastID = -1, start from highest number
            var ls = Database.DBGTS.GetTrades(index);
            var str = "";
            foreach(var o in ls)
            {
                str += JsonConvert.SerializeObject(o);
                str += "\r";
            }
            c.SendMessage(string.Format("<GTSREQUEST trades={0}>", str));
        }
        public static void OfferGTS(Client c,string pokemon, string idstr)
        {
            uint id;
            if (!uint.TryParse(idstr, out id))
                return;
            //If user is banned, return
            if (Database.DBUserChecks.UserBanned(c.Username))
            {
                return;
            }
            //Get Pokemon data from Database
            var poke = Database.DBGTS.GetSingleTrade(id);
            //If already accepted send negative response
            if (poke == null || poke.Accepted == true)
            {

                return;
            }
            //string to object
            var pkmn = JsonConvert.DeserializeObject<GamePokemon>(pokemon);
            var request = JsonConvert.DeserializeObject<RequestData>(poke.Request);
            //Compare pokemon offered with requests on pokemon with id offered.
            bool Correct = GTSCompare.ValidOffer(pkmn, request);
            //If doesn't match, send negative response
            if (Correct == false)
            {

            }
            //If it has been accepted, return negative (we do this twice to make absolutely sure it hasn't been traded in between)
            if (Database.DBGTS.GetAccepted(id))
            {

            }
            //Set accepted to true, Set pokemon offered as pokemon for collector to receive
            Database.DBGTS.SetAccepted(id, pokemon);
            //Send positive response along with pokemon back to client.

        }
        public static void GetUserTrades(Client c)
        {
            //Get a list of all the users trades
            var ls = Database.DBGTS.GetUserTrades(c.User_Id);
        }
        public static void CancelTrade(Client c, string idstr)
        {
            uint id;
            if (!uint.TryParse(idstr, out id))
                return;
            //make sure the user actually owns the trade
            if (!Database.DBGTS.UserOwnsTrade(id, c.User_Id))
            {
                return;
            }
            //Removes trade
            Database.DBGTS.CancelTrade(id);
        }
        public static void CollectTrade(Client c, string idstr)
        {
            uint id;
            if (!uint.TryParse(idstr, out id))
                return;
            //make sure the user actually owns the trade
            if (!Database.DBGTS.UserOwnsTrade(id, c.User_Id))
            {
                return;
            }
            //Collects the pokemon, and delete it from the database
            var poke = Database.DBGTS.CollectTrade(id);
            //Sends poke back to client

        }
    }
    public class RequestGTSHolder
    {
        public string Offer { get; set; }
        public string Request { get; set; }
        public bool Accepted { get; set; }
    }
}
