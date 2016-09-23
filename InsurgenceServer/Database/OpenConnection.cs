using MySql.Data.MySqlClient;

namespace InsurgenceServer.Database
{
    public class OpenConnection
    {
        private readonly string _connstring =
            $"Server=localhost; database={Data.Databasename}; UID={Auth.Username}; password={Auth.Password}";
        public MySqlConnection Connection;
        public OpenConnection()
        {
            Connection = new MySqlConnection(_connstring);
            Connection.Open();
        }
        public bool IsConnected()
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
            if (IsConnected())
            {
                Connection.Close();
            }
        }
    }
}
