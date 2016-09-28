using AdminSiteNew.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcSample.Web.Models;
using AdminSiteNew.DatabaseSpace;

namespace AdminSiteNew.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult AccessDenied()
        {
            return Redirect("/login");
        }
    }
}
