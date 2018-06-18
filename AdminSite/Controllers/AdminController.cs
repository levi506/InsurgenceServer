using System;
using System.Threading.Tasks;
using AdminSite.Database;
using AdminSite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AdminSite.Controllers
{
    [Authorize(Policy = "Administrator")]
    public class AdminController : Controller
    {
        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var model = new AdminModels.AdminModel()
            {
                 Users = await Database.DbAdmin.GetPermissions()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePermission ()
        {
            var newPermission = (AdminModels.PermissionsEnum)Enum.Parse(typeof(AdminModels.PermissionsEnum), Request.Form["user.Permission"]);
            if (newPermission == AdminModels.PermissionsEnum.Administrator)
            {
                return Redirect("/Admin/Index/");
            }
            if (Request.Form["user.ID"] == "117811387166947407528")
            {
                return Redirect("/Admin/Index/");
            }
            await Database.DbAdmin.UpdatePermissions(Request.Form["user.ID"], (int)newPermission);
            return Redirect("/Admin/Index/");
        }

        public async Task<IActionResult> Log()
        {
            var m = await DbAdminLog.GetLog();
            return View(m);
        }
    }
}

