using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcSample.Web.Models;

namespace MvcSample.Web
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public IActionResult Index()
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
