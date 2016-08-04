using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsurgenceServer.Database
{
    public class OpenConnection
    {
        private string connstring = string.Format("Server=localhost; database={0}; UID={1}; password={2}", Data.Databasename, Auth.Username, Auth.Password);
        public MySqlConnection Connection;
        public OpenConnection()
        {
            Connection = new MySqlConnection(connstring);
            Connection.Open();
        }
        public bool isConnected()
        {
            if (Connection.State == System.Data.ConnectionState.Open)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Close()
        {
            if (isConnected())
            {
                Connection.Close();
            }
        }
    }
}
