using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AdminSite.Controllers
{
    public class AccountController : Controller
    {
        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }
    }
}
