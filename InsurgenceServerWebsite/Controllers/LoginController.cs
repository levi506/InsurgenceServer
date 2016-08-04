using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using MvcSample.Web.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace InsurgenceServerWebsite.Controllers
{
    public class LoginController : Controller
    {
        // GET: /<controller>/
        public IActionResult Login()
        {
            return View();
        }
    }
}
