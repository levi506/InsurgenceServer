using System.Threading.Tasks;
using AdminSiteNew.Database;
using AdminSiteNew.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AdminSiteNew.Controllers
{
    [Authorize(Policy = "Moderator")]
    public class UsersController : Controller
    {
        public async Task<IActionResult> Users(string id)
        {
            if (id != "")
                GetUser(id);
            return View();
        }
        public async Task<IActionResult> UsersNum(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                return Redirect("/");
            var usr = await Database.Database.GetUsernameFromId(i);
            return Redirect("/Users/Users/" + usr);
        }
        

        [HttpPost]
        public async Task<IActionResult> FindUser(string request)
        {
            return Redirect("/Users/Users/" + request);
        }

        public async Task<PartialViewResult> GetUser(string username = "")
        {
            var req = username != "" ? await Database.Database.GetUser(username) : null;
            var model = new UserModel
            {
                request = username,
                UserRequest = req
            };
            return PartialView("UserHelper", model);
        }
        public async Task<IActionResult> BanAccount(string id, string redir = "")
        {
            await Database.Database.BanAccount(id, true);
            return redir != "" ? Redirect("/Users/Users/" + redir) : Redirect("/Users/Users/" + id);
        }
        public async Task<IActionResult> UnbanAccount(string id, string redir = "")
        {
            await Database.Database.BanAccount(id, false);
            return redir != "" ? Redirect("/Users/Users/" + redir) : Redirect("/Users/Users/" + id);
        }
        public async Task<IActionResult> BanIPs(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                return Redirect("/");
            await Database.Database.BanIPs(i, true);
            return Redirect("/Users/UsersNum/" + id);
        }
        public async Task<IActionResult> UnbanIPs(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                return Redirect("/");
            await Database.Database.BanIPs(i, false);
            return Redirect("/Users/UsersNum/" + id);
        }
        public async Task<IActionResult> BanSingleIp(string id, string name)
        {
            await Database.Database.BanSingleIp(id, true);
            return Redirect("/Users/Users/" + name);
        }
        public async Task<IActionResult> UnbanSingleIp(string id, string name)
        {
            await Database.Database.BanSingleIp(id, false);
            return Redirect("/Users/Users/" + name);
        }
        public async Task<IActionResult> BanAlts(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                return Redirect("/");
            await Database.Database.BanAlts(i, true);
            return Redirect("/Users/UsersNum/" + id);
        }
        public async Task<IActionResult> UnbanAlts(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                return Redirect("/");
            await Database.Database.BanAlts(i, false);
            return Redirect("/Users/UsersNum/" + id);
        }
    }

}
