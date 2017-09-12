using System;
using System.Linq;
using System.Threading.Tasks;
using AdminSiteNew.Auth;
using AdminSiteNew.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MvcSample.Web
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            Console.WriteLine(User.Claims.Count(x => x.Type == "Moderator"));
            return View(CreateUser());
        }

        public User CreateUser()
        {
            return new User()
            {
                Name = User.Identity.Name,
            };
        }
    }
}
