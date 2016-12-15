using AdminSiteNew.Database;
using AdminSiteNew.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcSample.Web.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AdminSiteNew.Controllers
{
    [Authorize(Policy = "Moderator")]
    public class UsersController : Controller
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
            var usr = Database.Database.GetUsernameFromID(i);
            return Redirect("/Users/Users/" + usr);
        }
        

        [HttpPost]
        public ActionResult FindUser(string request)
        {
            return Redirect("/Users/Users/" + request);
        }

        public PartialViewResult GetUser(string username = "")
        {
            var req = username != "" ? Database.Database.GetUser(username) : null;
            var model = new UserModel
            {
                request = username,
                UserRequest = req
            };
            return PartialView("UserHelper", model);
        }
        public ActionResult BanAccount(string id, string redir = "")
        {
            Database.Database.BanAccount(id, true);
            return redir != "" ? Redirect("/Users/Users/" + redir) : Redirect("/Users/Users/" + id);
        }
        public ActionResult UnbanAccount(string id, string redir = "")
        {
            Database.Database.BanAccount(id, false);
            return redir != "" ? Redirect("/Users/Users/" + redir) : Redirect("/Users/Users/" + id);
        }
        public ActionResult BanIPs(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                return Redirect("/");
            Database.Database.BanIPs(i, true);
            return Redirect("/Users/UsersNum/" + id);
        }
        public ActionResult UnbanIPs(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                return Redirect("/");
            Database.Database.BanIPs(i, false);
            return Redirect("/Users/UsersNum/" + id);
        }
        public ActionResult BanSingleIp(string id, string name)
        {
            Database.Database.BanSingleIp(id, true);
            return Redirect("/Users/Users/" + name);
        }
        public ActionResult UnbanSingleIp(string id, string name)
        {
            Database.Database.BanSingleIp(id, false);
            return Redirect("/Users/Users/" + name);
        }
        public ActionResult BanAlts(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                return Redirect("/");
            Database.Database.BanAlts(i, true);
            return Redirect("/Users/UsersNum/" + id);
        }
        public ActionResult UnbanAlts(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                return Redirect("/");
            Database.Database.BanAlts(i, false);
            return Redirect("/Users/UsersNum/" + id);
        }
    }

}
