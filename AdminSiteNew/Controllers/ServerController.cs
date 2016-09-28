using AdminSiteNew.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcSample.Web.Models;
using AdminSiteNew.DatabaseSpace;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AdminSiteNew.Controllers
{
    [Authorize(Policy = "Developer")]
    public class ServerController : Controller
    {
        public ActionResult Dashboard()
        {
            DashboardModel Model = new DashboardModel();
            return View(Model);
        }
        public PartialViewResult UserCount()
        {
            var Model = new DashboardModel();
            return PartialView("UserCount", Model);
        }
    }
}
