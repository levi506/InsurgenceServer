using System.Threading.Tasks;
using AdminSite.Database;
using AdminSite.Models;
using AdminSite.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AdminSite.Controllers
{
    [Authorize(Policy = "Moderator")]
    public class UsersController : Controller
    {
        public async Task<IActionResult> Users(string id)
        {
            if (id != "")
                await GetUser(id);
            return View("Users");
        }
        public async Task<IActionResult> UsersNum(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                return Redirect("/");
            var usr = await Database.Database.GetUsernameFromId(i);
            return await Users(usr);
        }


        [HttpPost]
        public async Task<IActionResult> FindUser(string request)
        {
            return Redirect("/Users/Users/" + request);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetUserApi(string id)
        {
            if (Request.Headers["api-token"] != Startup.Token)
            {
                return Forbid();
            }

            var req = id != null ? await Database.Database.GetUser(id.ToLower().StripSpecialCharacters()) : null;
            if (req == null)
            {
                return Json(null);
            }
            var model = new
            {
                username = req.UserInfo.Username,
                banned = req.UserInfo.Banned,
                friendsafari = req.FriendSafari.GetCleanPokemon
            };
            return Json(model);
        }

        public async Task<PartialViewResult> GetUser(string username = "")
        {
            var req = username != "" ? await Database.Database.GetUser(username.StripSpecialCharacters()) : null;
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
            DbAdminLog.Log(DbAdminLog.LogType.UserBan, User.Identity.Name, id);
            return redir != "" ? Redirect("/Users/Users/" + redir) : Redirect("/Users/Users/" + id);
        }
        public async Task<IActionResult> UnbanAccount(string id, string redir = "")
        {
            await Database.Database.BanAccount(id, false);
            DbAdminLog.Log(DbAdminLog.LogType.UserUnban, User.Identity.Name, id);
            return redir != "" ? Redirect("/Users/Users/" + redir) : Redirect("/Users/Users/" + id);
        }
        public async Task<IActionResult> BanIPs(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                return Redirect("/");
            await Database.Database.BanIPs(i, true);
            DbAdminLog.Log(DbAdminLog.LogType.IpBan, User.Identity.Name, id);
            return Redirect("/Users/UsersNum/" + id);
        }

        public async Task<IActionResult> UnbanIPs(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                return Redirect("/");
            await Database.Database.BanIPs(i, false);
            DbAdminLog.Log(DbAdminLog.LogType.IpUnban, User.Identity.Name, id);
            return Redirect("/Users/UsersNum/" + id);
        }
        public async Task<IActionResult> BanSingleIp(string id, string name)
        {
            await Database.Database.BanSingleIp(id, true);
            DbAdminLog.Log(DbAdminLog.LogType.IpBan, User.Identity.Name, id);
            return Redirect("/Users/Users/" + name);
        }
        public async Task<IActionResult> UnbanSingleIp(string id, string name)
        {
            await Database.Database.BanSingleIp(id, false);
            DbAdminLog.Log(DbAdminLog.LogType.IpUnban, User.Identity.Name, id);
            return Redirect("/Users/Users/" + name);
        }
        public async Task<IActionResult> BanAlts(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                return Redirect("/");
            await Database.Database.BanAlts(i, true);
            DbAdminLog.Log(DbAdminLog.LogType.AltsBan, User.Identity.Name, id);
            return Redirect("/Users/UsersNum/" + id);
        }
        public async Task<IActionResult> UnbanAlts(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                return Redirect("/");
            await Database.Database.BanAlts(i, false);
            DbAdminLog.Log(DbAdminLog.LogType.AltsUnban, User.Identity.Name, id);
            return Redirect("/Users/UsersNum/" + id);
        }

        public async Task<IActionResult> AddNote(UserNote note)
        {
            await DbUserNotes.AddNote(note.UserId, User.Identity.Name, note.Note);
            return Redirect("/Users/UsersNum/" + note.UserId);
        }
    }

}
