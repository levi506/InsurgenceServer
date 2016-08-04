using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using MvcSample.Web.Models;

namespace MvcSample.Web
{
    [Authorize(Policy = "Access")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(CreateUser());
        }

        public User CreateUser()
        {
            User user = new User()
            {
                Name = User.Identity.Name,
                Address = "My address"
            };

            return user;
        }
    }
}