using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AdminSite.Controllers
{
    public class LoginController : Controller
    {
        // GET: /<controller>/
        public async Task<IActionResult> Login()
        {
            return View();
        }
    }
}
