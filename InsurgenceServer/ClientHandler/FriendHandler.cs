using System.Threading.Tasks;

namespace InsurgenceServer.ClientHandler
{
    public static class FriendHandler
    {
        public static async Task AddFriend(Client client, string username)
        {
            if (client.Friends.Count >= Data.MaximumFriends)
            {
                //Already has 5 or more friends
                await client.SendMessage($"<ADDFRIEND user={username} result=0>");
                return;
            }
            var u = username.ToLower();
            //This is handled on the client side, only way this can happen is if someone messes with client code, so no graceful exit
            if (client.Username.ToLower() == u)
                return;
            if (!await Database.DbUserChecks.UserExists(username))
            {
                //User doesn't exist
                await client.SendMessage($"<ADDFRIEND user={username} result=1>");
                return;
            }
            if (await Database.DbUserChecks.UserBanned(username))
            {
                //User is banned
                await client.SendMessage($"<ADDFRIEND user={username} result=2>");
                return;
            }
            var c = ClientHandler.GetClient(u);
            if (c == null)
            {
                //User isn't online
                await client.SendMessage($"<ADDFRIEND user={username} result=3>");
                return;
            }
            if (client.Friends.Contains(c.UserId))
            {
                await client.SendMessage($"<ADDFRIEND user={username} result=4>");
            }
            client.PendingFriend = u;

            //Send the request to the other client
            await c.SendMessage($"<FRIENDREQ user={client.Username}>");
        }
        public static async Task AcceptRequest(Client client, string username)
        {
            var u = username.ToLower();

            var c = ClientHandler.GetClient(u);
            if (c.PendingFriend != client.Username.ToLower())
            {
                //Someone is messing with code, return ungracefully
                return;
            }
            if (client.Friends.Count >= Data.MaximumFriends)
            {
                //Can't accept if this client has more than 5 friends
                await c.SendMessage(($"<ADDFRIEND user={client.Username} result=5>"));
                return;
            }
            c.Friends.Add(client.UserId);
            client.Friends.Add(c.UserId);

            await Database.DbFriendHandler.UpdateFriends(c);
            await Database.DbFriendHandler.UpdateFriends(client);

            await c.SendMessage(($"<ADDFRIEND user={client.Username} result=7>"));
            await client.SendMessage(($"<ADDFRIEND user={client.Username} result=7>"));
        }
        public static async Task DenyRequest(Client client, string username)
        {
            var u = username.ToLower();

            var c = ClientHandler.GetClient(u);
            await c.SendMessage(($"<ADDFRIEND user={client.Username} result=6>"));
        }
        public static async Task RemoveFriend(Client client, string username)
        {
            var id = await Database.DbUserManagement.GetUserId(username);
            if (id == 0)
            {
                //that user does not exist
                await client.SendMessage($"FRIENDREMOVE user={username} result=0");
            }
            if (!client.Friends.Contains(id))
            {
                //Client does not have this user as friend, no need to remove
                await client.SendMessage($"FRIENDREMOVE user={username} result=1");
            }
            var c = ClientHandler.GetClient(id);
            if (c != null)
            {
                c.Friends.Remove(client.UserId);
                client.Friends.Remove(c.UserId);

                await Database.DbFriendHandler.UpdateFriends(client);
                await Database.DbFriendHandler.UpdateFriends(c);

                await client.SendMessage($"FRIENDREMOVE user={username} result=2");
                await c.SendMessage($"FRIENDREMOVE user={client.Username} result=2");

                return;
            }
            //Remove friend for user himself
            client.Friends.Remove(id);
            //Send the client a message about this so it can continue
            await client.SendMessage($"FRIENDREMOVE user={username} result=2");

            //Request the friends from other client from database
            var cl = await Database.DbFriendHandler.GetFriends(id);
            //Remove id, update on database
            cl.Remove(client.UserId);
            await Database.DbFriendHandler.UpdateFriends(id, cl);
        }

        public static async Task NotifyFriends(Client client, bool connected)
        {
            var clientList = ClientHandler.GetClientList(client.Friends);
            foreach (var friend in clientList)
            {
                try
                {
                    await friend.SendMessage(connected
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
