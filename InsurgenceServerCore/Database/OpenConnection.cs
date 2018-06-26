using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace InsurgenceServerCore.Database
{
    public class OpenConnection
    {
        private readonly string _connstring =
            $"Server=localhost; database={Data.Databasename}; UID={Auth.Username}; password={Auth.Password}; SslMode=none";
        public readonly MySqlConnection Connection;
        public OpenConnection()
        {
            Connection = new MySqlConnection(_connstring);
            Connection.Open();
        }
        public bool IsConnected()
        {
            return Connection.State == System.Data.ConnectionState.Open;
        }

        public async Task Close()
        {
            await Connection.CloseAsync();
        }
    }
}
