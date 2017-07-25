using System.Collections.Generic;
using System.Threading.Tasks;
using AdminSiteNew.Database;
using AdminSiteNew.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcSample.Web.Models;

namespace AdminSiteNew.Controllers
{
    [Authorize(Policy = "Developer")]
    public class ServerController : Controller
    {
        public ActionResult Dashboard()
        {
            if (ServerInteraction.Handler.Crashed)
            {
                ServerInteraction.Handler.Start();
            }
            DashboardModel Model = new DashboardModel();
            return View(Model);
        }
        public PartialViewResult UserCount()
        {
            var Model = new DashboardModel();
            return PartialView("UserCount", Model);
        }

        public async Task<IActionResult> Metrics()
        {
            var model = await DbMetrics.GetMetrics();
            return View(new MetricsHolder {List = model});
        }
    }
}
