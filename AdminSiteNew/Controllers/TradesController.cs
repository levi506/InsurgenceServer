using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminSiteNew.Database;
using AdminSiteNew.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AdminSiteNew.Controllers
{
    [Authorize(Policy = "Moderator")]
    public class TradesController : Controller
    {
        public async Task<IActionResult> RecentTrades(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                i = 0;
            var model = new RecentTradesModel
            {
                StartIndex = i,
                Trades = await DbTradelog.GetTradeLog(i)
            };
            return View(model);
        }

        public async Task<IActionResult> RecentWonderTrades(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                i = 0;
            var model = new RecentWonderTradesModel
            {
                StartIndex = i,
                Trades = await DbTradelog.GetWonderTradeLog(i)
            };
            return View(model);
        }

        public async Task<IActionResult> Tradelog(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                i = 0;
            return View(await DbTradelog.GetTrade(i));
        }
    }
}
