using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminSiteNew.Database;
using AdminSiteNew.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AdminSiteNew.Controllers
{
    [Authorize(Policy = "Administrator")]
    public class AdminController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            var model = new AdminModels.AdminModel()
            {
                 Users = Database.DbAdmin.GetPermissions()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePermission ()
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
            Database.DbAdmin.UpdatePermissions(Request.Form["user.ID"], (int)newPermission);
            return Redirect("/Admin/Index/");
        }
    }
}

