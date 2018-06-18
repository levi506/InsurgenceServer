using System.Threading.Tasks;
using AdminSite.Database;
using AdminSite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminSite.Controllers
{
    [Authorize(Policy = "Developer")]
    public class ServerController : Controller
    {
        public async Task<IActionResult> Dashboard()
        {
            if (ServerInteraction.Handler.Crashed)
            {
                ServerInteraction.Handler.Start();
            }
            DashboardModel Model = new DashboardModel();
            return View(Model);
        }
        public async Task<PartialViewResult> UserCount()
        {
            var model = new DashboardModel();
            return PartialView("UserCount", model);
        }

        public async Task<IActionResult> Metrics()
        {
            var model = await DbMetrics.GetMetrics();
            return View(new MetricsHolder {List = model});
        }
    }
}
