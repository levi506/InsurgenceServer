using System.Collections.Generic;
using System.Threading.Tasks;
using AdminSiteNew.Models;
using MySql.Data.MySqlClient;

namespace AdminSiteNew.Database
{
    internal static class DbAdmin
    {
        public static async Task<List<AdminModels.UserPermissions>> GetPermissions()
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected)
            { 
                conn.Close();
                return new List<AdminModels.UserPermissions>();
            }
            var l = new List<AdminModels.UserPermissions>();
            const string command = "SELECT id, access, name FROM newwebadmin";
            var mcom = new MySqlCommand(command, conn.Connection);
            using (var r = await mcom.ExecuteReaderAsync())
            {
                while (await r.ReadAsync())
                {
                    l.Add(new AdminModels.UserPermissions
                    {
                        Id = r["id"].ToString(),
                        Permission = (AdminModels.PermissionsEnum)r["access"],
                        Name = r["name"].ToString()
                    });
                }
            }
            conn.Close();
            return l;
        }

        public static async Task UpdatePermissions(string userid, int permission)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected)
            {
                conn.Close();
                return;
            }
            const string command = "UPDATE newwebadmin SET access= @access WHERE id= @id";
            var mcom = new MySqlCommand(command, conn.Connection);
            mcom.Parameters.AddWithValue("@id", userid);
            mcom.Parameters.AddWithValue("@access", permission);
            await mcom.ExecuteNonQueryAsync();
            conn.Close();
        }
    }
}
