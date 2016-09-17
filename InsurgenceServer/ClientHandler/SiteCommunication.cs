using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace InsurgenceServer
{
    public class SiteCommunication
    {
        private TcpClient _client;
        private NetworkStream stream;
        Byte[] bytes = new Byte[256];
        public SiteCommunication(TcpClient client)
        {
            Console.WriteLine("here");
            _client = client;
            stream = _client.GetStream();
            int i;
            while (Data.Running && (i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {

                var data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                Console.WriteLine(data);
                if (data.EndsWith("&"))
                    data = data.Remove(data.Length - 1);
                if (data.Contains("<INFO>"))
                {
                    GlobalInfoGetter();
                }
            }
        }
        private void GlobalInfoGetter()
        {
            var s = string.Format("<INFO\tusers={0}\ttrades={1}\tbattles={2}\tWT={3}>", ClientHandler.ActiveClients.Count, TradeHandler.ActiveTrades.Count,
                BattleHandler.ActiveBattles.Count, WonderTrade.WonderTradeHandler.List.Count);
            SendMessage(s);

        }
        private void UserInfoGetter()
        {
            var s = "";
            foreach (var user in ClientHandler.ActiveClients)
            {
                if (!user.LoggedIn)
                    continue;
                s += Utilities.Encoding.Base64Encode(user.Username);
                s += ",";
            }
            Console.WriteLine(s);
            SendMessage(s);
        }
        public void SendMessage(string str)
        {
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(str + "&");
            try
            {
                stream.Write(msg, 0, msg.Length);
                stream.Flush();
            }
            catch { }
        }
    }
}
