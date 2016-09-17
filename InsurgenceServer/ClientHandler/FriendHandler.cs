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
            //This is handled on the client side, only way this can happen is if someone messes with client code, so no graceful exit
            if (client.Username.ToLower() == u)
                return;
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
            if (client.Friends.Contains(c.User_Id))
            {
                client.SendMessage(string.Format("<ADDFRIEND user={0} result=4>", username));
            }
            client.PendingFriend = u;

            //Send the request to the other client
            c.SendMessage(string.Format("<FRIENDREQ user={0}>", client.Username));
        }
        public static void AcceptRequest(Client client, string username)
        {
            var u = username.ToLower();

            var c = ClientHandler.GetClient(u);
            if (c.PendingFriend != client.Username.ToLower())
            {
                //Someone is messing with code, return ungracefully
                return;
            }
            if (client.Friends.Count >= 5)
            {
                //Can't accept if this client has more than 5 friends
                c.SendMessage((string.Format("<ADDFRIEND user={0} result=5>", client.Username)));
                return;
            }
            c.Friends.Add(client.User_Id);
            client.Friends.Add(c.User_Id);

            Database.DBFriendHandler.UpdateFriends(c);
            Database.DBFriendHandler.UpdateFriends(client);

            c.SendMessage((string.Format("<ADDFRIEND user={0} result=7>", client.Username)));
            client.SendMessage((string.Format("<ADDFRIEND user={0} result=7>", client.Username)));
        }
        public static void DenyRequest(Client client, string username)
        {
            var u = username.ToLower();

            var c = ClientHandler.GetClient(u);
            c.SendMessage((string.Format("<ADDFRIEND user={0} result=6>", client.Username)));
        }
        public static void RemoveFriend(Client client, string username)
        {
            var id = Database.DBUserManagement.GetUserID(username);
            if (id == 0)
            {
                //that user does not exist
                client.SendMessage(string.Format("FRIENDREMOVE user={0} result=0", username));
            }
            if (!client.Friends.Contains(id))
            {
                //Client does not have this user as friend, no need to remove
                client.SendMessage(string.Format("FRIENDREMOVE user={0} result=1", username));
            }
            var c = ClientHandler.GetClient(id);
            if (c != null)
            {
                c.Friends.Remove(client.User_Id);
                client.Friends.Remove(c.User_Id);

                Database.DBFriendHandler.UpdateFriends(client);
                Database.DBFriendHandler.UpdateFriends(c);

                //TODO: Send messages to both clients about removed friends
                client.SendMessage(string.Format("FRIENDREMOVE user={0} result=2", username));
                c.SendMessage(string.Format("FRIENDREMOVE user={0} result=2", client.Username));

                return;
            }
            //Remove friend for user himself
            client.Friends.Remove(id);
            //Send the client a message about this so it can continue
            client.SendMessage(string.Format("FRIENDREMOVE user={0} result=2", username));

            //Request the friends from other client from database
            var cl = Database.DBFriendHandler.GetFriends(id);
            //Remove id, update on database
            cl.Remove(client.User_Id);
            Database.DBFriendHandler.UpdateFriends(id, cl);
        }

        public static async void NotifyFriends(Client client, bool Connected)
        {
            var clientList = ClientHandler.GetClientList(client.Friends);
            foreach (var friend in clientList)
            {
                try
                {
                    if (Connected)
                    {
                        friend.SendMessage(string.Format("<FRIENDCONNECT state=1 name={0}>", client.Username));
                    }
                    else
                    {
                        friend.SendMessage(string.Format("<FRIENDCONNECT state=0 name={0}>", client.Username));
                    }
                }
                catch { }
            }
        }
    }
}
