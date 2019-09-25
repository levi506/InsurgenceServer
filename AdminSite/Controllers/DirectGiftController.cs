using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdminSite.Database;
using AdminSite.Models;
using AdminSite.Pokemon;
using AdminSite.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AdminSite.Controllers
{
    [Authorize(Policy = "Developer")]
    public class DirectGiftController : Controller
    {
        public async Task<IActionResult> Index(string id)
        {
            if (id != "")
                await GetGifts(id);
            return View();
        }

        public async Task<PartialViewResult> GetGifts(string username = "")
        {
            var ls =  await DbDirectGifts.GetGifts(username.StripSpecialCharacters());
            var model = new DirectGiftModel
            {
                request = username,
                gifts = ls
            };
            return PartialView("DirectGiftTable", model);
        }

        [HttpPost]
        public ActionResult FindGifts(string request)
        {
            return Redirect("/DirectGift/Index/" + request);
        }

        public async Task<IActionResult> Details(string id)
        {
            var data = id.Split('?');
            var username = data[0];
            var giftIndex = int.Parse(data[1]);
            var newgift = bool.Parse(data[2]);
            var ls = await DbDirectGifts.GetGifts(username.StripSpecialCharacters());
            if (newgift)
            {
                var model = new DirectGiftDetailModel
                {
                    username = username,
                    gift = new DirectGiftBase(),
                    giftIndex = ls.Count
                };
                return View(model);
            }
            else
            {
                var model = new DirectGiftDetailModel
                {
                    username = username,
                    gift = ls[giftIndex],
                    giftIndex = giftIndex
                };
                return View(model);
            }
        }

        public class GiftModel
        {
            public string Type { get; set; }
            public string Username { get; set; }
            public Models.Pokemon Pokemon { get; set; }
            public int? Item { get; set; }
            public int? ItemAmount { get; set; }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AddGiftApi([FromBody]GiftModel model)
        {
            if (string.Equals(Request.Headers["api-token"], Startup.Token, StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Wrong token given: " + Request.Headers["api-token"]);
                return Forbid();
            }

            var username = model.Username.StripSpecialCharacters();
            var ls = await DbDirectGifts.GetGifts(username);
            if (model.Type == "pokemon")
            {
                model.Pokemon.exp = GrowthRates.CalculateExp(model.Pokemon.species, model.Pokemon.level);
                var rand        = new Random();
                var id = rand.Next(int.MinValue, int.MaxValue);
                model.Pokemon.personalID = (uint) id;
                model.Pokemon.timeReceived = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var gift = new PokemonDirectGift()
                {
                    Pokemon = model.Pokemon,
                    Username = username,
                    Index =  ls.Count
                };
                ls.Add(gift);
                await DbDirectGifts.SetDirectGifts(username, ls);
                DbAdminLog.Log(DbAdminLog.LogType.DirectGiftPokemon, "Bot Endpoint", JsonConvert.SerializeObject(gift));
            }
            else if (model.Type == "item")
            {
                var item = model.Item.Value;
                var amount = model.ItemAmount.Value;
                var gift = new ItemDirectGift()
                {
                    Amount = (uint) amount,
                    Index = ls.Count,
                    Username = username,
                    Item = (ItemList) item
                };
                ls.Add(gift);
                await DbDirectGifts.SetDirectGifts(username, ls);
                DbAdminLog.Log(DbAdminLog.LogType.DirectGiftItem, "Bot Endpoint", JsonConvert.SerializeObject(gift));
            }
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> AddPokemonGift()
        {
            var otgender = int.Parse(Request.Form["pokemonModel.Pokemon.otgender"]);
            if (otgender == 2) otgender = 0;
            var pkmn = new Models.Pokemon
            {
                species = short.Parse(Request.Form["pokemonModel.Pokemon.species"]),
                name = Request.Form["pokemonModel.Pokemon.name"].ToString(),
                ot = Request.Form["pokemonModel.Pokemon.ot"].ToString(),
                isShiny = bool.Parse(Request.Form["pokemonModel.Pokemon.isShiny"][0]),
                level = byte.Parse(Request.Form["pokemonModel.Pokemon.level"]),
                gender = byte.Parse(Request.Form["pokemonModel.Pokemon.gender"]),
                nature = byte.Parse(Request.Form["pokemonModel.Pokemon.nature"]),
                happiness = int.Parse(Request.Form["pokemonModel.Pokemon.happiness"]),
                item = int.Parse(Request.Form["pokemonModel.Pokemon.item"]),
                iv = new[]
                {
                    int.Parse(Request.Form["pokemonModel.Pokemon.iv[0]"]),
                    int.Parse(Request.Form["pokemonModel.Pokemon.iv[1]"]),
                    int.Parse(Request.Form["pokemonModel.Pokemon.iv[2]"]),
                    int.Parse(Request.Form["pokemonModel.Pokemon.iv[3]"]),
                    int.Parse(Request.Form["pokemonModel.Pokemon.iv[4]"]),
                    int.Parse(Request.Form["pokemonModel.Pokemon.iv[5]"])
                },
                ev = new[]
                {
                    int.Parse(Request.Form["pokemonModel.Pokemon.ev[0]"]),
                    int.Parse(Request.Form["pokemonModel.Pokemon.ev[1]"]),
                    int.Parse(Request.Form["pokemonModel.Pokemon.ev[2]"]),
                    int.Parse(Request.Form["pokemonModel.Pokemon.ev[3]"]),
                    int.Parse(Request.Form["pokemonModel.Pokemon.ev[4]"]),
                    int.Parse(Request.Form["pokemonModel.Pokemon.ev[5]"])
                },
                moves = new[] {
                    new GameMove
                    {
                        id = int.Parse(Request.Form["pokemonModel.Pokemon.moves[0].id"]),
                        pp = int.Parse(Request.Form["pokemonModel.Pokemon.moves[0].pp"]),
                        ppup = int.Parse(Request.Form["pokemonModel.Pokemon.moves[0].ppup"]),
                    },
                    new GameMove
                    {
                        id = int.Parse(Request.Form["pokemonModel.Pokemon.moves[1].id"]),
                        pp = int.Parse(Request.Form["pokemonModel.Pokemon.moves[1].pp"]),
                        ppup = int.Parse(Request.Form["pokemonModel.Pokemon.moves[1].ppup"]),
                    },
                    new GameMove
                    {
                        id = int.Parse(Request.Form["pokemonModel.Pokemon.moves[2].id"]),
                        pp = int.Parse(Request.Form["pokemonModel.Pokemon.moves[2].pp"]),
                        ppup = int.Parse(Request.Form["pokemonModel.Pokemon.moves[2].ppup"]),
                    },
                    new GameMove
                    {
                        id = int.Parse(Request.Form["pokemonModel.Pokemon.moves[3].id"]),
                        pp = int.Parse(Request.Form["pokemonModel.Pokemon.moves[3].pp"]),
                        ppup = int.Parse(Request.Form["pokemonModel.Pokemon.moves[3].ppup"]),
                    }
                },
                otgender = (byte) otgender,
                obtainMode = 4
            };
            pkmn.exp = GrowthRates.CalculateExp(pkmn.species, pkmn.level);

            var rand = new Random();
            var id = rand.Next(int.MinValue, int.MaxValue);
            pkmn.personalID = (uint) id;
            var data = PokemonDatabase.GetData(pkmn.species);
            var abil = byte.Parse(Request.Form["pokemonModel.Pokemon.ability"]);
            if (data.Abilities.Count < 2 && abil == 1)
            {
                abil = 0;
            }
            if (data.HiddenAbility.Count < 1 && abil == 2)
            {
                abil = 0;
            }
            pkmn.ability = abil;
            pkmn.timeReceived = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var username = Request.Form["username"].ToString().StripSpecialCharacters();
            var ls = await DbDirectGifts.GetGifts(username);
            var index = int.Parse(Request.Form["giftIndex"]);
            var gift = new PokemonDirectGift
            {
                Pokemon = pkmn,
                Index = index,
                Username = username,
            };
            if (index >= ls.Count)
            {
                ls.Add(gift);
            }
            else
            {
                ls[index] = gift;
            }

            await DbDirectGifts.SetDirectGifts(username, ls);
            DbAdminLog.Log(DbAdminLog.LogType.DirectGiftPokemon, User.Identity.Name, JsonConvert.SerializeObject(gift));


            return Redirect("/DirectGift/Index/" + username);
        }

        public async Task<IActionResult> DeleteGift(string username, string index)
        {
            var i = int.Parse(index);
            var ls = await DbDirectGifts.GetGifts(username.StripSpecialCharacters());
            ls.RemoveAt(i);
            await DbDirectGifts.SetDirectGifts(username.StripSpecialCharacters(), ls);
            DbAdminLog.Log(DbAdminLog.LogType.DirectGiftDelete, User.Identity.Name, username);
            return Redirect("/DirectGift/Index/" + username.StripSpecialCharacters());
        }

        [HttpPost]
        public async Task<IActionResult> AddItemGift()
        {

            var username = Request.Form["username"].ToString().StripSpecialCharacters();
            var ls = await DbDirectGifts.GetGifts(username);
            var index = int.Parse(Request.Form["giftIndex"]);
            var gift = new ItemDirectGift
            {
                Item = (ItemList)Enum.Parse(typeof(ItemList), Request.Form["itemModel.Item"]),
                Amount = uint.Parse(Request.Form["itemModel.Amount"]),
                Index =index,
                Username = username
            };

            if (index >= ls.Count)
            {
                ls.Add(gift);
            }
            else
            {
                ls[index] = gift;
            }
            await DbDirectGifts.SetDirectGifts(username, ls);
            DbAdminLog.Log(DbAdminLog.LogType.DirectGiftItem, User.Identity.Name, JsonConvert.SerializeObject(gift));

            return Redirect("/DirectGift/Index/" + username);
        }
    }
}
