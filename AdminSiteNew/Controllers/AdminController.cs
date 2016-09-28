using AdminSiteNew.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcSample.Web.Models;
using AdminSiteNew.DatabaseSpace;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AdminSiteNew.Controllers
{
    [Authorize(Policy = "Moderator")]
    public class AdminController : Controller
    {
        public ActionResult Users(string id)
        {
            if (id != "")
                GetUser(id);
            return View();
        }
        public ActionResult UsersNum(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                return Redirect("/");
            var usr = Database.GetUsernameFromID(i);
            return Redirect("/Admin/Users/" + usr);
        }
        public ActionResult RecentTrades(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                i = 0;
            RecentTradesModel Model = new RecentTradesModel();
            Model.StartIndex = i;
            Model.Trades = Database.GetTradeLog(i);
            return View(Model);
        }

        [HttpPost]
        public ActionResult FindUser(string request)
        {
            return Redirect("/Admin/Users/" + request);
        }

        public PartialViewResult GetUser(string username = "")
        {
            UserModel Model = new UserModel();
            UserRequest req;
            if (username != "")
            {
                req = Database.GetUser(username);
            }
            else
            {
                req = null;
            }
            Model.request = username;
            Model.UserRequest = req;
            return PartialView("UserHelper", Model);
        }
        public ActionResult BanAccount(string id, string redir = "")
        {
            Database.BanAccount(id, true);
            if (redir != "")
                return Redirect("/Admin/Users/" + redir);
            return Redirect("/Admin/Users/" + id);
        }
        public ActionResult UnbanAccount(string id, string redir = "")
        {
            Database.BanAccount(id, false);
            if (redir != "")
                return Redirect("/Admin/Users/" + redir);
            return Redirect("/Admin/Users/" + id);
        }
        public ActionResult BanIPs(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                return Redirect("/");
            Database.BanIPs(i, true);
            return Redirect("/Admin/UsersNum/" + id);
        }
        public ActionResult UnbanIPs(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                return Redirect("/");
            Database.BanIPs(i, false);
            return Redirect("/Admin/UsersNum/" + id);
        }
        public ActionResult BanSingleIp(string id, string name)
        {
            Database.BanSingleIp(id, true);
            return Redirect("/Admin/Users/" + name);
        }
        public ActionResult UnbanSingleIp(string id, string name)
        {
            Database.BanSingleIp(id, false);
            return Redirect("/Admin/Users/" + name);
        }
        public ActionResult BanAlts(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                return Redirect("/");
            Database.BanAlts(i, true);
            return Redirect("/Admin/UsersNum/" + id);
        }
        public ActionResult UnbanAlts(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                return Redirect("/");
            Database.BanAlts(i, false);
            return Redirect("/Admin/UsersNum/" + id);
        }
    }

}
