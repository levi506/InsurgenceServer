using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AdminSiteNew.Controllers
{
    [Authorize(Policy = "Moderator")]
    public class GTSController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
