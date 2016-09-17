using InsurgenceServerWebsite.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using MvcSample.Web.Models;
using InsurgenceServerWebsite.DatabaseSpace;
using InsurgenceServerAdmin.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace InsurgenceServerWebsite.Controllers
{
    [Authorize(Policy = "Deuk")]
    public class ServerController : Controller
    {
        public ActionResult Dashboard()
        {
            DashboardModel Model = new DashboardModel();
            Model.Users = InsurgenceServerAdmin.ServerInteraction.Handler.Users;
            return View(Model);
        }

    }
}
