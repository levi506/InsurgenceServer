using System.Collections.Generic;
using AdminSiteNew.Database;
using AdminSiteNew.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcSample.Web.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

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

        public ActionResult Metrics()
        {
            var model = DbMetrics.GetMetrics();
            return View(new MetricsHolder {List = model});
        }
    }
}
