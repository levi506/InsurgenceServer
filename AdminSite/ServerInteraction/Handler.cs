using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace AdminSite.ServerInteraction
{
    public static class Handler
    {
        public static int UserCount;
        public static int TradeCount;
        public static int BattleCount;
        public static int WTCount;

        public static bool Crashed = false;

        private static TcpClient client;
        private static NetworkStream stream;
        private static readonly Byte[] bytes = new Byte[256];

        public static bool IsConnected()
        {
            return client.Connected;
        }

        public static void Start()
        {
            client = new TcpClient();
            try
            {
                var T = client.ConnectAsync("127.0.0.1", 6419);
                T.Wait();
            }
            catch (AggregateException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Server is not online");
                Console.ResetColor();
                Crashed = true;
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Crashed = true;
                return;
            }
            Console.WriteLine("Connected to local server");
            stream = client.GetStream();
            new System.Threading.Thread(RealStart
            ).Start();
        }
        private static void RealStart()
        {
            while (client.Connected)
            {
                try
                {
                    var s = SendMessage("<INFO>");
                    var command = new CommandHandler(s);
                    UserCount = int.Parse(command.Data["users"]);
                    TradeCount = int.Parse(command.Data["trades"]);
                    BattleCount = int.Parse(command.Data["battles"]);
                    WTCount = int.Parse(command.Data["WT"]);

                    System.Threading.Thread.Sleep(1000);
                }
                catch
                {

                }
                /*
                var UsersEnc = s.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var u in UsersEnc)
                {
                    if (u == "" || u == "\n" || u == null || u == " ")
                        continue;
                    Users.Add(Utilities.Encoding.Base64Decode(u));
                }
                Console.WriteLine(Users.Count);
                */
            }
        }
        private static string SendMessage(string message)
        {
            var asen = new ASCIIEncoding();
            var ba = asen.GetBytes(message + "&");
            stream.Write(ba, 0, ba.Length);
            stream.Flush();

            var bb = new byte[255];

            var i = stream.Read(bb, 0, bb.Length);
            var data = Encoding.ASCII.GetString(bb, 0, i);
            var s = "";
            while (true)
            {
                s += data;
                if (data.Contains("&"))
                {
                    break;
                }
            }
            s = s.Remove(data.Length - 1);
            return s;
        }
        private static void MessageReceiver()
        {
            int i;
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                try
                {
                    var data = Encoding.ASCII.GetString(bytes, 0, i);
                    DataHandler(data);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static string _message = "";
        private static void DataHandler(string data)
        {
            if (data.EndsWith("&"))
                data = data.Remove(data.Length - 1);
            _message += data;
            if (!data.EndsWith(">"))
            {
                return;
            }
            var command = new CommandHandler(_message);
            _message = "";


        }
    }

    public class CommandHandler
    {
        public Commands Command;
        public Dictionary<string, string> Data = new Dictionary<string, string>();
        public CommandHandler(string _input)
        {
            if (!(_input.StartsWith("<") && _input.EndsWith(">")))
                return;
            var input = _input.Remove(0, 1);
            input = input.Remove(input.Length - 1);
            var arr = input.Split('\t');
            if (!Enum.TryParse<Commands>(arr[0], out Command))
            {
                Console.WriteLine("Unexpected Command: " + arr[0]);
                return;
            }
            for (var i = 1; i < arr.Length; i++)
            {
                if (arr[i] == "")
                    continue;
                var carr = arr[i].Split('=');
                var arg = "";
                for (var j = 1; j < carr.Length; j++)
                {
                    arg += carr[j];
                    if (j != 1)
                        arg += "=";
                }
                Data.Add(carr[0], arg);
            }
        }
    }
    public enum Commands
    {
        Null = 0, INFO
    }
}
