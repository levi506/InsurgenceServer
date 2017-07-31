using System.Threading.Tasks;
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
            Initialization = Connection.OpenAsync();
        }
        public bool IsConnected()
        {
            return Connection.State == System.Data.ConnectionState.Open;
        }
        public Task Initialization { get; private set; }

        public async Task Close()
        {
            if (IsConnected())
            {
                await Connection.CloseAsync();
            }
        }
    }
}
