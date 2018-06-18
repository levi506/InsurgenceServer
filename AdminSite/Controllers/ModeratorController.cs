using System.Threading.Tasks;
using AdminSite.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminSite.Controllers
{
    [Authorize(Policy = "Moderator")]
    public class ModeratorController : Controller
    {
        public async Task<IActionResult> Warnings()
        {
            var l = await DBWarnings.GetMetrics();
            return View(l);
        }
    }
}