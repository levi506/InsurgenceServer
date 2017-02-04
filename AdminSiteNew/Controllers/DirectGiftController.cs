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
        public ActionResult Index(string id)
        {
            Console.WriteLine("username: " + id);
            if (id != "")
                GetGifts(id);
            return View();
        }

        public PartialViewResult GetGifts(string username = "")
        {
            var ls = DBDirectGifts.GetGifts(username);
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
            Console.WriteLine(request);
            return Redirect("/DirectGift/Index/" + request);
        }

        public ActionResult Details(string id)
        {
            var data = id.Split('?');
            var username = data[0];
            var giftIndex = int.Parse(data[1]);
            var newgift = bool.Parse(data[2]);
            var ls = DBDirectGifts.GetGifts(username);
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
        public ActionResult AddPokemonGift()
        {
            var pkmn = new Pokemon
            {
                species = short.Parse(Request.Form["pokemonModel.Pokemon.species"]),
                name = Request.Form["pokemonModel.Pokemon.name"].ToString(),
                ot = Request.Form["pokemonModel.Pokemon.ot"].ToString(),
                isShiny = bool.Parse(Request.Form["pokemonModel.Pokemon.isShiny"][0]),
                level = byte.Parse(Request.Form["pokemonModel.Pokemon.level"])
            };

            var username = Request.Form["username"].ToString();
            var ls = DBDirectGifts.GetGifts(username);
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

            DBDirectGifts.SetDirectGifts(username, ls);


            return Redirect("/DirectGift/Index/" + username);
        }

        public ActionResult DeleteGift(string username, string index)
        {
            var Index = int.Parse(index);
            var ls = DBDirectGifts.GetGifts(username);
            ls.RemoveAt(Index);
            DBDirectGifts.SetDirectGifts(username, ls);
            return Redirect("/DirectGift/Index/" + username);
        }

        [HttpPost]
        public ActionResult AddItemGift()
        {

            var username = Request.Form["username"].ToString();
            var ls = DBDirectGifts.GetGifts(username);
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
            DBDirectGifts.SetDirectGifts(username, ls);

            return Redirect("/DirectGift/Index/" + username);
        }
    }
}
