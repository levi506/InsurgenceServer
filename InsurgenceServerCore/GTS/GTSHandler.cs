using System;
using System.Threading.Tasks;
using InsurgenceServerCore.ClientHandler;
using InsurgenceServerCore.Trades;
using Newtonsoft.Json;

namespace InsurgenceServerCore.GTS
{
    public static class GtsHandler
    {
        public static async Task CreateGts(Client c,string offer, string request, string index)
        {
            //If user is banned, return
            if (await Database.DbUserChecks.UserBanned(c.Username))
            {
                await c.SendMessage($"<GTSCREATE result=0 index={index}>");
                return;
            }
            //If user has more than max amount of allowed GTS trades, return
            if (await Database.Dbgts.GetNumberOfTrades(c.UserId) >= Data.MaximumGtsTradesPerUser)
            {
                await c.SendMessage($"<GTSCREATE result=1 index={index}>");
                return;
            }
            //Decode data
            var decodeOffer = Utilities.Encoding.Base64Decode(offer);
            var decodeRequest = Utilities.Encoding.Base64Decode(request);

            int level;
            try
            {
                //Turn data into objects
                var pokemon = JsonConvert.DeserializeObject<GamePokemon>(decodeOffer);

                if (!await TradeValidator.IsPokemonValid(pokemon, c.UserId))
                {
                    await c.SendMessage($"<GTSCREATE result=0 index={index}>");
                    return;
                }

                //Get Pokemon Level
                level = GrowthRates.CalculateLevel(pokemon.species, pokemon.exp);
            }
            catch (InvalidCastException)
            {
                await c.SendMessage($"<GTSCREATE result=2 index={index}>");
                return;
            }

            //Input in database
            try
            {
                await Database.Dbgts.Add(c.UserId, decodeOffer, decodeRequest, level, c.Username);
                await c.SendMessage($"<GTSCREATE result=3 index={index}>");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await c.SendMessage($"<GTSCREATE result=2 index={index}>");
            }
        }
        public static async Task RequestGts(Client c, string lastIDstr, string filterstring)
        {
            uint index;
            if (!uint.TryParse(lastIDstr, out index))
            {
                Console.WriteLine("Index is wrong");
                return;
            }
            var filter = JsonConvert.DeserializeObject<FilterHolder>(Utilities.Encoding.Base64Decode(filterstring));
            //Request pokemon from the database, starting with pokemon last seen + 1. If lastID = -1, start from highest number
            var ls = await Database.Dbgts.GetTrades(index, filter);
            var str = "";
            foreach(var o in ls)
            {
                str += JsonConvert.SerializeObject(o);
                str += "\r";
            }
            //We encode the string with base64 for neater transmitting and less chance of user inputs breaking things
            var compressed = Utilities.Encoding.Base64Encode(str);
            await c.SendMessage($"<GTSREQUEST trades={compressed}>");
        }
        public static async Task OfferGts(Client c,string pokemon, string idstr)
        {
            uint id;
            if (!uint.TryParse(idstr, out id))
                return;
            //If user is banned, return
            if (await Database.DbUserChecks.UserBanned(c.Username))
            {
                await c.SendMessage("<GTSOFFER result=0 pkmn=nil>");
                return;
            }
            //Get Pokemon data from Database
            var poke = await Database.Dbgts.GetSingleTrade(id);
            //If already accepted send negative response
            if (poke == null || poke.Accepted)
            {
                await c.SendMessage("<GTSOFFER result=1 pkmn=nil>");
                return;
            }
            //decode string
            var decoded = Utilities.Encoding.Base64Decode(pokemon);
            //string to object
            var pkmn = JsonConvert.DeserializeObject<GamePokemon>(decoded);

            if (!await TradeValidator.IsPokemonValid(pkmn, c.UserId))
            {
                await c.SendMessage("<GTSOFFER result=0 pkmn=nil>");
                return;
            }


            var request = poke.Request;
            //Compare pokemon offered with requests on pokemon with id offered.
            var correct = GtsCompare.ValidOffer(pkmn, request);
            //If doesn't match, send negative response
            if (correct == false)
            {
                await c.SendMessage("<GTSOFFER result=2 pkmn=nil>");
                return;
            }
            //If it has been accepted, return negative (we do this twice to make absolutely sure it hasn't been traded in between)
            if (await Database.Dbgts.GetAccepted(id))
            {
                await c.SendMessage("<GTSOFFER result=1 pkmn=nil>");
                return;
            }
            //Set accepted to true, Set pokemon offered as pokemon for collector to receive
            await Database.Dbgts.SetAccepted(id, decoded);
            //Send positive response along with pokemon back to client.
            var encodedPokemon = Utilities.Encoding.Base64Encode(JsonConvert.SerializeObject(poke.Offer));
            await c.SendMessage($"<GTSOFFER result=3 pkmn={encodedPokemon}>");
        }
        public static async Task GetUserTrades(Client c)
        {
            //Get a list of all the users trades
            var ls = await Database.Dbgts.GetUserTrades(c.UserId);

            //List to string, then send
            var str = "";
            foreach (var o in ls)
            {
                str += JsonConvert.SerializeObject(o);
                str += "\r";
            }
            var encoded = Utilities.Encoding.Base64Encode(str);
            await c.SendMessage($"<GTSMINE trades={encoded}>");
        }
        public static async Task CancelTrade(Client c, string idstr)
        {
            uint id;
            if (!uint.TryParse(idstr, out id))
                return;
            //make sure the user actually owns the trade
            if (!await Database.Dbgts.UserOwnsTrade(id, c.UserId))
            {
                await c.SendMessage("<GTSCANCEL result=0 pkmn=nil>");
                return;
            }
            //Get actual trade
            var trade = await Database.Dbgts.GetSingleTrade(id);
            //Return if trade is already accepted
            if (trade.Accepted)
            {
                await c.SendMessage("<GTSCANCEL result=1 pkmn=nil>");
                return;
            }
            //Removes trade
            await Database.Dbgts.CancelTrade(id);
            //Return offered pokemon to the client
            var encodedPokemon = Utilities.Encoding.Base64Encode(JsonConvert.SerializeObject(trade.Offer));
            await c.SendMessage($"<GTSCANCEL result=2 pkmn={encodedPokemon}>");
        }
        public static async Task CollectTrade(Client c, string idstr)
        {
            if (!uint.TryParse(idstr, out var id))
            {
                Console.WriteLine($"User tried to collect pokemon with invalid id: {idstr}");
                return;
            }
            //make sure the user actually owns the trade
            if (!await Database.Dbgts.UserOwnsTrade(id, c.UserId))
            {
                Console.WriteLine($"User tried to collect pokemon he didn't own. User-id: {c.UserId}, Trade id: {id}");
                await c.SendMessage("<GTSCOLLECT result=0 pkmn=nil>");
                return;
            }
            //make sure the trade is actually accepted
            if (!await Database.Dbgts.TradeIsAccepted(id))
            {
                Console.WriteLine($"User tried to collect not accepted trade. User-id: {c.UserId}, Trade id: {id}");
                await c.SendMessage("<GTSCOLLECT result=1 pkmn=nil>");
                return;
            }
            //Collects the pokemon, and delete it from the database
            var poke = await Database.Dbgts.CollectTrade(id);
            //Sends poke back to client
            var encodedPokemon = Utilities.Encoding.Base64Encode(poke);
            await c.SendMessage($"<GTSCOLLECT result=2 pkmn={encodedPokemon}>");
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
