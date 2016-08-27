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
            //Decode data
            var decodeOffer = Utilities.Encoding.Base64Decode(Offer);
            var decodeRequest = Utilities.Encoding.Base64Decode(Request);
            //Input in database
            try
            {
                Database.DBGTS.Add(c.User_Id, decodeOffer, decodeRequest);
                c.SendMessage(string.Format("<GTSCREATE result=3 index={0}>", index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                c.SendMessage(string.Format("<GTSCREATE result=2 index={0}>", index));
            }
        }
        public static void RequestGTS(Client c, string lastIDstr)
        {
            uint index;
            if (!uint.TryParse(lastIDstr, out index))
            {
                Console.WriteLine("NAN");
                return;
            }
            //Request pokemon from the database, starting with pokemon last seen + 1. If lastID = -1, start from highest number
            var ls = Database.DBGTS.GetTrades(index);
            var str = "";
            foreach(var o in ls)
            {
                str += JsonConvert.SerializeObject(o);
                str += "\r";
            }
            //We encode the string with base64 for neater transmitting and less chance of user inputs breaking things
            var compressed = Utilities.Encoding.Base64Encode(str);
            c.SendMessage(string.Format("<GTSREQUEST trades={0}>", compressed));
        }
        public static void OfferGTS(Client c,string pokemon, string idstr)
        {
            uint id;
            if (!uint.TryParse(idstr, out id))
                return;
            //If user is banned, return
            if (Database.DBUserChecks.UserBanned(c.Username))
            {
                c.SendMessage(string.Format("<GTSOFFER result=0 pkmn=nil>"));
                return;
            }
            //Get Pokemon data from Database
            var poke = Database.DBGTS.GetSingleTrade(id);
            //If already accepted send negative response
            if (poke == null || poke.Accepted == true)
            {
                c.SendMessage(string.Format("<GTSOFFER result=1 pkmn=nil>"));
                return;
            }
            //decode string
            var decoded = Utilities.Encoding.Base64Decode(pokemon);
            //string to object
            var pkmn = JsonConvert.DeserializeObject<GamePokemon>(decoded);
            var request = poke.Request;
            //Compare pokemon offered with requests on pokemon with id offered.
            bool Correct = GTSCompare.ValidOffer(pkmn, request);
            //If doesn't match, send negative response
            if (Correct == false)
            {
                c.SendMessage(string.Format("<GTSOFFER result=2 pkmn=nil>"));
                return;
            }
            //If it has been accepted, return negative (we do this twice to make absolutely sure it hasn't been traded in between)
            if (Database.DBGTS.GetAccepted(id))
            {
                c.SendMessage(string.Format("<GTSOFFER result=1 pkmn=nil>"));
                return;
            }
            //Set accepted to true, Set pokemon offered as pokemon for collector to receive
            Database.DBGTS.SetAccepted(id, pokemon);
            //Send positive response along with pokemon back to client.
            var encodedPokemon = Utilities.Encoding.Base64Encode(JsonConvert.SerializeObject(poke.Offer));
            c.SendMessage(string.Format("<GTSOFFER result=3 pkmn={0}>", encodedPokemon));
        }
        public static void GetUserTrades(Client c)
        {
            //Get a list of all the users trades
            var ls = Database.DBGTS.GetUserTrades(c.User_Id);

            //List to string, then send
            var str = "";
            foreach (var o in ls)
            {
                str += JsonConvert.SerializeObject(o);
                str += "\r";
            }
            var encoded = Utilities.Encoding.Base64Encode(str);
            c.SendMessage(string.Format("<GTSMINE trades={0}>", encoded));
        }
        public static void CancelTrade(Client c, string idstr)
        {
            uint id;
            if (!uint.TryParse(idstr, out id))
                return;
            //make sure the user actually owns the trade
            if (!Database.DBGTS.UserOwnsTrade(id, c.User_Id))
            {
                c.SendMessage("<GTSCANCEL result=0 pkmn=nil>");
                return;
            }
            //Get actual trade
            var trade = Database.DBGTS.GetSingleTrade(id);
            //Return if trade is already accepted
            if (trade.Accepted)
            {
                c.SendMessage("<GTSCANCEL result=1 pkmn=nil>");
                return;
            }
            //Removes trade
            Database.DBGTS.CancelTrade(id);
            //Return offered pokemon to the client
            var encodedPokemon = Utilities.Encoding.Base64Encode(JsonConvert.SerializeObject(trade.Offer));
            c.SendMessage(string.Format("<GTSCANCEL result=2 pkmn={0}>", encodedPokemon));
        }
        public static void CollectTrade(Client c, string idstr)
        {
            uint id;
            if (!uint.TryParse(idstr, out id))
                return;
            //make sure the user actually owns the trade
            if (!Database.DBGTS.UserOwnsTrade(id, c.User_Id))
            {
                c.SendMessage("<GTSCOLLECT result=0 pkmn=nil>");
                return;
            }
            //make sure the trade is actually accepted
            if (!Database.DBGTS.TradeIsAccepted(id))
            {
                c.SendMessage("<GTSCOLLECT result=1 pkmn=nil>");
                return;
            }
            //Collects the pokemon, and delete it from the database
            var poke = Database.DBGTS.CollectTrade(id);
            //Sends poke back to client
            var encodedPokemon = Utilities.Encoding.Base64Encode(poke);
            c.SendMessage(string.Format("<GTSCOLLECT result=2 pkmn={0}>", encodedPokemon));
        }
    }
    public class RequestGTSHolder
    {
        public int Index { get; set; }
        public GamePokemon Offer { get; set; }
        public RequestData Request { get; set; }
        public bool Accepted { get; set; }
    }
}
