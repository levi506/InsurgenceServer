using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminSite.Database;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminSite.Models
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

        public class AdminLogModel
        {
            public uint Id { get; set; }
            public string Moderator { get; set; }
            public DbAdminLog.LogType Type { get; set; }
            public string Data { get; set; }
        }
    }
}
