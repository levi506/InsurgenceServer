using System.Threading.Tasks;
using AdminSite.Database;
using AdminSite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AdminSite.Controllers
{
    [Authorize(Policy = "Moderator")]
    public class GTSController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var model = new GTSListModel
            {
                GTS = await DbGTS.GetOpenGTSTrades(),
                StartIndex = 0
            };
            return View(model);
        }

        public async Task<IActionResult> Closed()
        {
            var model = new GTSListModel()
            {
                GTS =  await DbGTS.GetClosedGTSTrades(),
                StartIndex = 0
            };
            return View(model);
        }

        public  async Task<IActionResult> Detail(string id)
        {
            int i;
            if (!int.TryParse(id, out i))
                i = 0;
            var model = await DbGTS.GetSingleGTSTrade(i);
            return View(model);
        }

        public async Task<IActionResult> Remove(string id)
        {
            int i;
            if (!int.TryParse(id, out i))
                return BadRequest();
            await DbGTS.DeleteGTS(i);
            DbAdminLog.Log(DbAdminLog.LogType.GtsRemove, User.Identity.Name, id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RemoveUserTrades(string id)
        {
            uint i;
            if (!uint.TryParse(id, out i))
                return BadRequest();
            await DbGTS.RemoveUserGTS(i);
            DbAdminLog.Log(DbAdminLog.LogType.GtsRemoveUser, User.Identity.Name, id);
            return RedirectToAction("Index");
        }
    }
}
