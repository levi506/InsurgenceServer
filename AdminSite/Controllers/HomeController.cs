using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminSite.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            Console.WriteLine(User.Claims.Count(x => x.Type == "Moderator"));
            ViewData["name"] = User.Identity.Name;
            return View();
        }
    }
}
