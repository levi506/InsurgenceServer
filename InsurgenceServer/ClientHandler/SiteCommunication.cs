using System;
using System.Net.Sockets;
using System.Text;
using InsurgenceServer.Battles;

namespace InsurgenceServer
{
    public class SiteCommunication
    {
        private readonly NetworkStream _stream;
        readonly Byte[] _bytes = new Byte[256];
        public SiteCommunication(TcpClient client)
        {
            Console.WriteLine("here");
            _stream = client.GetStream();
            int i;
            while (Data.Running && (i = _stream.Read(_bytes, 0, _bytes.Length)) != 0)
            {

                var data = Encoding.ASCII.GetString(_bytes, 0, i);
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
            var s =
                $"<INFO\tusers={ClientHandler.ActiveClients.Count}\ttrades={TradeHandler.ActiveTrades.Count}\tbattles={BattleHandler.ActiveBattles.Count}\tWT={WonderTrade.WonderTradeHandler.List.Count}>";
            SendMessage(s);

        }

        public void SendMessage(string str)
        {
            byte[] msg = Encoding.ASCII.GetBytes(str + "&");
            try
            {
                _stream.Write(msg, 0, msg.Length);
                _stream.Flush();
            }
            catch
            {
                // ignored
            }
        }
    }
}
