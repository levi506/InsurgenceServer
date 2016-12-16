using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Serialization;

namespace InsurgenceServer.Database
{
    public static class DbFriendSafari
    {
        public static void GetBase(string username, Client client)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
            {
                conn.Close();
                return;
            }
            const string command = "SELECT friendsafari.base, friendsafari.message, users.banned " +
                                   "FROM users " +
                                   "INNER JOIN friendsafari " +
                                   "ON friendsafari.user_id = users.user_id " +
                                   "WHERE users.username = @param_val_1;";
            var m = new MySqlCommand(command, conn.Connection);
            m.Parameters.AddWithValue("@param_val_1", username);
            var result = m.ExecuteReader();
            if (!result.HasRows)
            {
                //We just send the login message so we don't need a special client handler just for this. User doesn't notice anyway
                client.SendMessage("<LOG result=0>");
                conn.Close();
                return;
            }
            while (result.Read())
            {
                if ((bool)result["banned"])
                {
                    client.SendMessage($"<TRA user={username} result=1>");
                    conn.Close();
                    return;
                }
                if (result["base"] is DBNull)
                {
                    client.SendMessage($"<VBASE user={username} result=1 base=nil>");
                    conn.Close();
                    return;
                }
                var Base = result["friendsafari.base"];
                
                var messageDb = result["friendsafari.message"];
                var message = String.Empty;
                if (messageDb is DBNull)
                {
                    message = "nil";
                }
                else
                {
                    message = messageDb.ToString();
                }
                conn.Close();
                client.SendMessage($"<VBASE user={username} result=2 base={Base} message={message}>");
                return;
            }
            conn.Close();
        }
        public static void UploadBase(uint userId, string Base)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected())
            {
                const string command = "INSERT INTO friendsafari (user_id, base) " +
                                       "VALUES (@param_val_2, @param_val_1) " +
                                       "ON DUPLICATE KEY " +
                                       "UPDATE " +
                                       "base = VALUES(@param_val_1)";
                var m = new MySqlCommand(command, conn.Connection);
                m.Parameters.AddWithValue("@param_val_1", Base);
                m.Parameters.AddWithValue("@param_val_2", userId);
                m.ExecuteNonQuery();
            }
            conn.Close();
        }

        public static void SetMessage(uint userId, string message)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected())
            {
                const string command = "UPDATE friendsafari SET message = @param_val_1 WHERE user_id = param_val_2";
                var m = new MySqlCommand(command, conn.Connection);
                m.Parameters.AddWithValue("@param_val_1", message);
                m.Parameters.AddWithValue("@param_val_2", userId);
                m.ExecuteNonQuery();
            }
            conn.Close();
        }

        public static void RemoveMessage(uint userId)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected())
            {
                const string command = "UPDATE friendsafari SET message = NULL WHERE user_id = param_val_2";
                var m = new MySqlCommand(command, conn.Connection);
                m.Parameters.AddWithValue("@param_val_2", userId);
                m.ExecuteNonQuery();
            }
            conn.Close();
        }
        /// <summary>
        /// Add a gift to a giftbox
        /// </summary>
        /// <returns>0 if something went wrong, 1 if gift can't be given, 2 if giftbox is full, 3 if success</returns>
        public static int AddGift(Client client, uint gift, string username)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
                return 0;

            if (!VerifyGift(gift))
            {
                return 1;
            }

            //Grab current gifts and the user id from database
            const string command = "SELECT user_id, giftbox " +
                                   "FROM friendsafari " +
                                   "INNER JOIN users " +
                                   "ON users.user_id = friendsafari.user_id " +
                                   "WHERE users.username = @param_val_1";
            var m = new MySqlCommand(command, conn.Connection);
            m.Parameters.AddWithValue("@param_val_1", username);
            var r = m.ExecuteReader();

            uint userId = 0;
            var oldGifts = string.Empty;

            while (r.Read())
            {
                userId = (uint)r["user_id"];
                var tempGifts = r["giftbox"];
                if (!(tempGifts is DBNull))
                {
                    oldGifts = tempGifts.ToString();
                }
            }

            //This should never happen, as when a gift is given, the base should exist
            //If it does happen however, ungraceful quit
            if (userId == 0)
                return 0;

            //Turn the string we got from the database into a List of unsigned integers
            var giftsList = oldGifts.Split(',').Select(x => new GiftHolder(x)).ToList();

            if (giftsList.Count >= Data.MaximumGifts)
            {
                return 2;
            }

            var giftObj = giftsList.FirstOrDefault(x => x.ItemId == gift);

            if (giftObj == null)
            {
                giftsList.Add(new GiftHolder {ItemId = gift, Amount = 1});
            }
            else
            {
                giftObj.Amount++;
            }

            var newGifts = string.Join(",", giftsList.Select(x => x.ToString()));

            const string insertCommand = "UPDATE friendsafari SET giftbox = @param_val_1 WHERE user_id = @param_val_2";
            var im = new MySqlCommand(insertCommand, conn.Connection);
            im.Parameters.AddWithValue("@param_val_1", newGifts);
            im.Parameters.AddWithValue("@param_val_2", userId);

            im.ExecuteNonQuery();

            return 3;
        }

        public static void GetGifts(Client client)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
                return;

            //Get the gifts
            const string command = "SELECT giftbox FROM friendsafari WHERE user_id = @param_val_1";
            var m = new MySqlCommand(command, conn.Connection);
            m.Parameters.AddWithValue("@param_val_1", client.UserId);
            var gifts = m.ExecuteScalar().ToString();

            //Send the gifts
            client.SendMessage($"<FSGIFTS gifts={gifts}>");

            //Remove the gifts from the database
            const string removeCommand = "UPDATE friendsafari SET giftbox = NULL WHERE user_id = @param_val_1";
            var rm = new MySqlCommand(removeCommand, conn.Connection);
            rm.Parameters.AddWithValue("@param_val_1", client.UserId);
            rm.ExecuteNonQuery();
        }

        public static bool VerifyGift(uint gift)
        {
            //TODO actually write this
            return true;
        }

        public class GiftHolder
        {
            public uint ItemId { get; set; }
            public uint Amount { get; set; }

            public GiftHolder(string s)
            {
                var l = s.Split(':');
                ItemId = uint.Parse(l[0]);
                Amount = uint.Parse(l[1]);
            }

            public GiftHolder() { }

            public override string ToString()
            {
                return $"{ItemId}:{Amount}";
            }
        }
    }
}
