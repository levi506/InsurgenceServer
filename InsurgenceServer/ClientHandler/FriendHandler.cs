namespace InsurgenceServer
{
    public static class FriendHandler
    {
        public static void AddFriend(Client client, string username)
        {
            if (client.Friends.Count >= 5)
            {
                //Already has 5 or more friends
                client.SendMessage($"<ADDFRIEND user={username} result=0>");
                return;
            }
            var u = username.ToLower();
            //This is handled on the client side, only way this can happen is if someone messes with client code, so no graceful exit
            if (client.Username.ToLower() == u)
                return;
            if (!Database.DbUserChecks.UserExists(username))
            {
                //User doesn't exist
                client.SendMessage($"<ADDFRIEND user={username} result=1>"); 
                return;
            }
            if (Database.DbUserChecks.UserBanned(username))
            {
                //User is banned
                client.SendMessage($"<ADDFRIEND user={username} result=2>");
                return;
            }
            var c = ClientHandler.GetClient(u);
            if (c == null)
            {
                //User isn't online
                client.SendMessage($"<ADDFRIEND user={username} result=3>");
                return;
            }
            if (client.Friends.Contains(c.UserId))
            {
                client.SendMessage($"<ADDFRIEND user={username} result=4>");
            }
            client.PendingFriend = u;

            //Send the request to the other client
            c.SendMessage($"<FRIENDREQ user={client.Username}>");
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
                c.SendMessage(($"<ADDFRIEND user={client.Username} result=5>"));
                return;
            }
            c.Friends.Add(client.UserId);
            client.Friends.Add(c.UserId);

            Database.DbFriendHandler.UpdateFriends(c);
            Database.DbFriendHandler.UpdateFriends(client);

            c.SendMessage(($"<ADDFRIEND user={client.Username} result=7>"));
            client.SendMessage(($"<ADDFRIEND user={client.Username} result=7>"));
        }
        public static void DenyRequest(Client client, string username)
        {
            var u = username.ToLower();

            var c = ClientHandler.GetClient(u);
            c.SendMessage(($"<ADDFRIEND user={client.Username} result=6>"));
        }
        public static void RemoveFriend(Client client, string username)
        {
            var id = Database.DbUserManagement.GetUserId(username);
            if (id == 0)
            {
                //that user does not exist
                client.SendMessage($"FRIENDREMOVE user={username} result=0");
            }
            if (!client.Friends.Contains(id))
            {
                //Client does not have this user as friend, no need to remove
                client.SendMessage($"FRIENDREMOVE user={username} result=1");
            }
            var c = ClientHandler.GetClient(id);
            if (c != null)
            {
                c.Friends.Remove(client.UserId);
                client.Friends.Remove(c.UserId);

                Database.DbFriendHandler.UpdateFriends(client);
                Database.DbFriendHandler.UpdateFriends(c);

                //TODO: Send messages to both clients about removed friends
                client.SendMessage($"FRIENDREMOVE user={username} result=2");
                c.SendMessage($"FRIENDREMOVE user={client.Username} result=2");

                return;
            }
            //Remove friend for user himself
            client.Friends.Remove(id);
            //Send the client a message about this so it can continue
            client.SendMessage($"FRIENDREMOVE user={username} result=2");

            //Request the friends from other client from database
            var cl = Database.DbFriendHandler.GetFriends(id);
            //Remove id, update on database
            cl.Remove(client.UserId);
            Database.DbFriendHandler.UpdateFriends(id, cl);
        }

        public static void NotifyFriends(Client client, bool connected)
        {
            var clientList = ClientHandler.GetClientList(client.Friends);
            foreach (var friend in clientList)
            {
                try
                {
                    friend.SendMessage(connected
                        ? $"<FRIENDCONNECT state=1 name={client.Username}>"
                        : $"<FRIENDCONNECT state=0 name={client.Username}>");
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}
