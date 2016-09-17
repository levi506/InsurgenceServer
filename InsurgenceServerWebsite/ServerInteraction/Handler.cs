using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace InsurgenceServerAdmin.ServerInteraction
{
    public static class Handler
    {
        public static List<string> Users = new List<string>();
        private static TcpClient client;
        private static NetworkStream stream;
        private static Byte[] bytes = new Byte[256];

        public static void Start()
        {
            client = new TcpClient("127.0.0.1", 6419);
            stream = client.GetStream();
            new System.Threading.Thread(() =>
               RealStart()
            ).Start();
        }
        private static void RealStart()
        {
            while (true)
            {
                string s = SendMessage("<INFO>");
                Console.WriteLine(s);
                var UsersEnc = s.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var u in UsersEnc)
                {
                    if (u == "" || u == "\n" || u == null || u == " ")
                        continue;
                    Users.Add(Utilities.Encoding.Base64Decode(u));
                }
                Console.WriteLine(Users.Count);

            }
        }
        private static string SendMessage(string message)
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] ba = asen.GetBytes(message + "&");
            stream.Write(ba, 0, ba.Length);
            stream.Flush();

            byte[] bb = new byte[255];

            var i = stream.Read(bb, 0, bb.Length);
            var data = System.Text.Encoding.ASCII.GetString(bb, 0, i);
            string s = "";
            bool collecting = true;
            while (collecting)
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
                    var data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    DataHandler(data);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static string Message = "";
        private static void DataHandler(string data)
        {
            if (data.EndsWith("&"))
                data = data.Remove(data.Length - 1);
            Message += data;
            if (!data.EndsWith(">"))
            {
                return;
            }
            var command = new CommandHandler(Message);
            Message = "";


        }
    }

    public class CommandHandler
    {
        public Commands Command;
        public Dictionary<string, string> data = new Dictionary<string, string>();
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
                data.Add(carr[0], arg);
            }
        }
    }
    public enum Commands
    {
        Null = 0, INFO
    }
}
