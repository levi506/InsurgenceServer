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
        public ActionResult RecentTrades(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                i = 0;
            var model = new RecentTradesModel
            {
                StartIndex = i,
                Trades = DbTradelog.GetTradeLog(i)
            };
            return View(model);
        }

        public ActionResult RecentWonderTrades(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                i = 0;
            var model = new RecentWonderTradesModel
            {
                StartIndex = i,
                Trades = DbTradelog.GetWonderTradeLog(i)
            };
            return View(model);
        }

        public ActionResult Tradelog(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                i = 0;
            return View(DbTradelog.GetTrade(i));
        }
    }
}
