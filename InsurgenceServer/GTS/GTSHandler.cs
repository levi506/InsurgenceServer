using Newtonsoft.Json;
using System;

namespace InsurgenceServer.GTS
{
    public static class GtsHandler
    {
        public static void CreateGts(Client c,string offer, string request, string index)
        {
            //If user is banned, return
            if (Database.DbUserChecks.UserBanned(c.Username))
            {
                c.SendMessage($"<GTSCREATE result=0 index={index}>");
                return;
            }
            //If user has more than max amount of allowed GTS trades, return
            if (Database.Dbgts.GetNumberOfTrades(c.UserId) >= Data.MaximumGtsTradesPerUser)
            {
                c.SendMessage($"<GTSCREATE result=1 index={index}>");
                return;
            }
            //Decode data
            var decodeOffer = Utilities.Encoding.Base64Decode(offer);
            var decodeRequest = Utilities.Encoding.Base64Decode(request);

            //Turn data into objects
            var pokemon = JsonConvert.DeserializeObject<GamePokemon>(decodeOffer);

            //Get Pokemon Level
            var level = GrowthRates.CalculateLevel(pokemon.species, pokemon.exp);

            //Input in database
            try
            {
                Database.Dbgts.Add(c.UserId, decodeOffer, decodeRequest, level, c.Username);
                c.SendMessage($"<GTSCREATE result=3 index={index}>");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                c.SendMessage($"<GTSCREATE result=2 index={index}>");
            }
        }
        public static void RequestGts(Client c, string lastIDstr, string filterstring)
        {
            uint index;
            if (!uint.TryParse(lastIDstr, out index))
            {
                Console.WriteLine("NAN");
                return;
            }
            var filter = JsonConvert.DeserializeObject<FilterHolder>(Utilities.Encoding.Base64Decode(filterstring));
            Console.WriteLine(filter.Species);
            //Request pokemon from the database, starting with pokemon last seen + 1. If lastID = -1, start from highest number
            var ls = Database.Dbgts.GetTrades(index, filter);
            var str = "";
            foreach(var o in ls)
            {
                str += JsonConvert.SerializeObject(o);
                str += "\r";
            }
            //We encode the string with base64 for neater transmitting and less chance of user inputs breaking things
            var compressed = Utilities.Encoding.Base64Encode(str);
            c.SendMessage($"<GTSREQUEST trades={compressed}>");
        }
        public static void OfferGts(Client c,string pokemon, string idstr)
        {
            uint id;
            if (!uint.TryParse(idstr, out id))
                return;
            //If user is banned, return
            if (Database.DbUserChecks.UserBanned(c.Username))
            {
                c.SendMessage("<GTSOFFER result=0 pkmn=nil>");
                return;
            }
            //Get Pokemon data from Database
            var poke = Database.Dbgts.GetSingleTrade(id);
            //If already accepted send negative response
            if (poke == null || poke.Accepted)
            {
                c.SendMessage("<GTSOFFER result=1 pkmn=nil>");
                return;
            }
            //decode string
            var decoded = Utilities.Encoding.Base64Decode(pokemon);
            //string to object
            var pkmn = JsonConvert.DeserializeObject<GamePokemon>(decoded);
            var request = poke.Request;
            //Compare pokemon offered with requests on pokemon with id offered.
            var correct = GtsCompare.ValidOffer(pkmn, request);
            //If doesn't match, send negative response
            if (correct == false)
            {
                c.SendMessage("<GTSOFFER result=2 pkmn=nil>");
                return;
            }
            //If it has been accepted, return negative (we do this twice to make absolutely sure it hasn't been traded in between)
            if (Database.Dbgts.GetAccepted(id))
            {
                c.SendMessage("<GTSOFFER result=1 pkmn=nil>");
                return;
            }
            //Set accepted to true, Set pokemon offered as pokemon for collector to receive
            Database.Dbgts.SetAccepted(id, pokemon);
            //Send positive response along with pokemon back to client.
            var encodedPokemon = Utilities.Encoding.Base64Encode(JsonConvert.SerializeObject(poke.Offer));
            c.SendMessage($"<GTSOFFER result=3 pkmn={encodedPokemon}>");
        }
        public static void GetUserTrades(Client c)
        {
            //Get a list of all the users trades
            var ls = Database.Dbgts.GetUserTrades(c.UserId);

            //List to string, then send
            var str = "";
            foreach (var o in ls)
            {
                str += JsonConvert.SerializeObject(o);
                str += "\r";
            }
            var encoded = Utilities.Encoding.Base64Encode(str);
            c.SendMessage($"<GTSMINE trades={encoded}>");
        }
        public static void CancelTrade(Client c, string idstr)
        {
            uint id;
            if (!uint.TryParse(idstr, out id))
                return;
            //make sure the user actually owns the trade
            if (!Database.Dbgts.UserOwnsTrade(id, c.UserId))
            {
                c.SendMessage("<GTSCANCEL result=0 pkmn=nil>");
                return;
            }
            //Get actual trade
            var trade = Database.Dbgts.GetSingleTrade(id);
            //Return if trade is already accepted
            if (trade.Accepted)
            {
                c.SendMessage("<GTSCANCEL result=1 pkmn=nil>");
                return;
            }
            //Removes trade
            Database.Dbgts.CancelTrade(id);
            //Return offered pokemon to the client
            var encodedPokemon = Utilities.Encoding.Base64Encode(JsonConvert.SerializeObject(trade.Offer));
            c.SendMessage($"<GTSCANCEL result=2 pkmn={encodedPokemon}>");
        }
        public static void CollectTrade(Client c, string idstr)
        {
            uint id;
            if (!uint.TryParse(idstr, out id))
                return;
            //make sure the user actually owns the trade
            if (!Database.Dbgts.UserOwnsTrade(id, c.UserId))
            {
                c.SendMessage("<GTSCOLLECT result=0 pkmn=nil>");
                return;
            }
            //make sure the trade is actually accepted
            if (!Database.Dbgts.TradeIsAccepted(id))
            {
                c.SendMessage("<GTSCOLLECT result=1 pkmn=nil>");
                return;
            }
            //Collects the pokemon, and delete it from the database
            var poke = Database.Dbgts.CollectTrade(id);
            //Sends poke back to client
            var encodedPokemon = Utilities.Encoding.Base64Encode(poke);
            c.SendMessage($"<GTSCOLLECT result=2 pkmn={encodedPokemon}>");
        }
    }
    public class RequestGtsHolder
    {
        public int Index { get; set; }
        public GamePokemon Offer { get; set; }
        public RequestData Request { get; set; }
        public bool Accepted { get; set; }
    }
}
