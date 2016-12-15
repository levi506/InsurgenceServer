using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminSiteNew.Models
{
    public class AdminModels
    {
        public class AdminModel
        {
            public List<UserPermissions> Users { get; set; }
        }

        public class UserPermissions
        {
            public string Id { get; set; }
            public PermissionsEnum Permission { get; set; }
            public string Name { get; set; }
        }

        public class PermissionChangeModel
        {
            public string Id { get; set; }
        }

        public enum PermissionsEnum
        {
            User,
            Moderator,
            Developer,
            Administrator
        }
    }
}
