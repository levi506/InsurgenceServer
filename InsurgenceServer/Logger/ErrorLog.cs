using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsurgenceServer.Logger
{
    public static class ErrorLog
    {
        private static DiscordClient _client;
        private static Channel errorchannel;

        public static void Initialize()
        {
            _client = new DiscordClient();
            _client.ExecuteAndWait(async () => {
                await _client.Connect(DiscordAuth.Secret);
                errorchannel = _client.GetChannel(DiscordAuth.ErrorChannel);
            });
        }
        public static void Log(Exception e)
        {
            if (errorchannel == null)
            {
                try
                {
                    errorchannel = _client.GetChannel(DiscordAuth.ErrorChannel);
                }
                catch
                {
                    errorchannel = null;
                }
            }
            if (errorchannel == null)
                return;
            try
            {
                errorchannel.SendMessage(e.ToString());
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
        }
    }
}
