using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminSiteNew.Database;
using AdminSiteNew.Models;
using AdminSiteNew.PokemonHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AdminSiteNew.Controllers
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
            var ls =  await DbDirectGifts.GetGifts(username);
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
            var ls = await DbDirectGifts.GetGifts(username);
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

        [HttpPost]
        public async Task<IActionResult> AddPokemonGift()
        {
            var otgender = int.Parse(Request.Form["pokemonModel.Pokemon.otgender"]);
            if (otgender == 2) otgender = 0;
            var pkmn = new Pokemon
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
                moves = new List<GameMove>
                {
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
                    },
                },
                otgender = otgender,
                obtainMode = 4
            };
            pkmn.exp = GrowthRates.CalculateExp(pkmn.species, pkmn.level);

            var rand = new Random();
            var id = (byte)rand.Next(256);
            id |= (byte)(((byte) rand.Next(256)) << 8);
            id |= (byte)(((byte)rand.Next(256)) << 16);
            id |= (byte)(((byte)rand.Next(256)) << 24);
            pkmn.personalID = id;
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

            var username = Request.Form["username"].ToString();
            var ls = await DbDirectGifts.GetGifts(username);
            var index = int.Parse(Request.Form["giftIndex"]);
            var gift = new PokemonDirectGift
            {
                Type = DirectGiftType.Pokemon,
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


            return Redirect("/DirectGift/Index/" + username);
        }

        public async Task<IActionResult> DeleteGift(string username, string index)
        {
            var i = int.Parse(index);
            var ls = await DbDirectGifts.GetGifts(username);
            ls.RemoveAt(i);
            await DbDirectGifts.SetDirectGifts(username, ls);
            return Redirect("/DirectGift/Index/" + username);
        }

        [HttpPost]
        public async Task<IActionResult> AddItemGift()
        {

            var username = Request.Form["username"].ToString();
            var ls = await DbDirectGifts.GetGifts(username);
            var index = int.Parse(Request.Form["giftIndex"]);
            var gift = new ItemDirectGift
            {
                Type = DirectGiftType.Item,
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

            return Redirect("/DirectGift/Index/" + username);
        }
    }
}
