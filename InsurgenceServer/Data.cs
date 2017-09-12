using System.Collections.ObjectModel;
using System.Net.Sockets;

namespace InsurgenceServer
{
    public static class Data
    {
        public const string Ip = "5.135.154.100";
        public const int Port = 6421;
        public const string Databasename = "insurgence";
        public const double ServerVersion = 3.1;
        public const int MaximumConnections = 200;

        public const int MaximumGtsTradesPerUser = 3;
        public const int MaximumFriends = 5;

        public const int MaximumGifts = 50;

        public static bool AcceptingConnections = true;
        public static bool Running = true;

        public static TcpListener Server;
        public static TcpListener SiteServer;

        public static ObservableCollection<string> Log = new ObservableCollection<string>();

        public const bool TradeDisabled = false;

        public static readonly string[] BannedOTs = {
            "godnemesis", "nemesis"
        };

        public static readonly uint[] BannedTrainerIDs = {
            341946228
        };
    }

}

