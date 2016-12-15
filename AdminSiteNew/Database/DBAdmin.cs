using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminSiteNew.Models;
using MySql.Data.MySqlClient;

namespace AdminSiteNew.Database
{
    internal static class DbAdmin
    {
        public static List<AdminModels.UserPermissions> GetPermissions()
        {
            var conn = new OpenConnection();
            if (!conn.isConnected())
            {
                conn.Close();
                return new List<AdminModels.UserPermissions>();
            }
            var l = new List<AdminModels.UserPermissions>();
            const string command = "SELECT id, access, name FROM webadmin";
            var mcom = new MySqlCommand(command, conn.Connection);
            var r = mcom.ExecuteReader();
            while (r.Read())
            {
                l.Add(new AdminModels.UserPermissions
                {
                    Id = r["id"].ToString(),
                    Permission = (AdminModels.PermissionsEnum)r["access"],
                    Name = r["name"].ToString()
                });
            }
            return l;
        }

        public static void UpdatePermissions(string userid, int permission)
        {
            var conn = new OpenConnection();
            if (!conn.isConnected())
            {
                conn.Close();
                return;
            }
            const string command = "UPDATE webadmin SET access= @access WHERE id= @id";
            var mcom = new MySqlCommand(command, conn.Connection);
            mcom.Parameters.AddWithValue("@id", userid);
            mcom.Parameters.AddWithValue("@access", permission);
            mcom.ExecuteNonQuery();
        }
    }
}
