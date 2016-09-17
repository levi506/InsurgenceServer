using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsurgenceServer
{
    public static class FriendHandler
    {
        public static void AddFriend(Client client, string username)
        {
            if (client.Friends.Count >= 5)
            {
                //Already has 5 or more friends
                client.SendMessage(string.Format("<ADDFRIEND user={0} result=0>", username));
                return;
            }
            var u = username.ToLower();
            if (!Database.DBUserChecks.UserExists(username))
            {
                //User doesn't exist
                client.SendMessage(string.Format("<ADDFRIEND user={0} result=1>", username)); 
                return;
            }
            if (Database.DBUserChecks.UserBanned(username))
            {
                //User is banned
                client.SendMessage(string.Format("<ADDFRIEND user={0} result=2>", username));
                return;
            }
            var c = ClientHandler.GetClient(u);
            if (c == null)
            {
                //User isn't online
                client.SendMessage(string.Format("<ADDFRIEND user={0} result=3>", username));
                return;
            }

            //Send the request to the other client
            c.SendMessage(string.Format("<FRIENDREQ user={0}>", client.Username));

            return;
        }
    }
}
