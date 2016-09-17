using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;

namespace InsurgenceServer
{
	public static class Data
	{
		public const string IP = "5.135.154.100";
		public const int Port = 6421;
		public const string Databasename = "insurgence";
		public const double ServerVersion = 3.0;
		public const int MaximumConnections = 200;

        public const int MaximumGTSTradesPerUser = 3;

		public static bool AcceptingConnections = true;
		public static bool Running = true;

		public static TcpListener Server;
        public static TcpListener SiteServer;

        public static ObservableCollection<string> log = new ObservableCollection<string>();
	}
    
}

