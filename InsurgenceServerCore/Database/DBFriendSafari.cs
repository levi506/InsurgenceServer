using System;
using System.Linq;
using System.Threading.Tasks;
using InsurgenceServerCore.ClientHandler;
using MySql.Data.MySqlClient;

namespace InsurgenceServerCore.Database
{
    public static class DbFriendSafari
    {
        public static async Task GetBase(string username, Client client)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
            {
                await conn.Close();
                return;
            }
            const string command = "SELECT friendsafari.base, friendsafari.message, users.banned " +
                                   "FROM users " +
                                   "INNER JOIN friendsafari " +
                                   "ON friendsafari.user_id = users.user_id " +
                                   "WHERE users.username = @param_val_1;";
            var m = new MySqlCommand(command, conn.Connection);
            m.Parameters.AddWithValue("@param_val_1", username);
            var result = await m.ExecuteReaderAsync();
            if (!result.HasRows)
            {
                //We just send the login message so we don't need a special client handler just for this. User doesn't notice anyway
                await client.SendMessage("<LOG result=0>");
                await conn.Close();
                return;
            }
            while (await result.ReadAsync())
            {
                if ((bool)result["banned"])
                {
                    await client.SendMessage($"<TRA user={username} result=1>");
                    await conn.Close();
                    return;
                }
                if (result["base"] is DBNull)
                {
                    await client.SendMessage($"<VBASE user={username} result=1 base=nil>");
                    await conn.Close();
                    return;
                }
                var Base = result["base"];

                var messageDb = result["message"];
                var message = messageDb is DBNull ? "nil" : messageDb.ToString();
                await client.SendMessage($"<VBASE user={username} result=2 base={Base} message={Utilities.Encoding.Base64Encode(message)}>");
                break;
            }
            await conn.Close();
        }

        public static async Task GetRandomBase(Client client)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
            {
                await conn.Close();
                return;
            }
            const string command = "SELECT friendsafari.base, friendsafari.message, users.username, users.banned " +
                                   "FROM friendsafari " +
                                   "INNER JOIN users " +
                                   "ON friendsafari.user_id = users.user_id " +
                                   "WHERE friendsafari.base IS NOT NULL " +
                                   "AND friendsafari.base <> '' " +
                                   "AND users.banned <> 1 " +
                                   "ORDER BY RAND() " +
                                   "LIMIT 1";
            var m = new MySqlCommand(command, conn.Connection);
            var result = await m.ExecuteReaderAsync();
            while (await result.ReadAsync())
            {
                var Base = result["base"];

                var messageDb = result["message"];
                var username = result["username"];
                var message = messageDb is DBNull ? "nil" : messageDb.ToString();
                await client.SendMessage(
                    $"<VBASE user={username} result=2 base={Base} message={Utilities.Encoding.Base64Encode(message)}>");
                break;
            }
            await conn.Close();
        }


        public static async Task UploadBase(uint userId, string Base)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected())
            {
                const string command = "INSERT INTO friendsafari (user_id, base) " +
                                       "VALUES (@param_val_2, @param_val_1) " +
                                       "ON DUPLICATE KEY " +
                                       "UPDATE " +
                                       "base = @param_val_1";
                var m = new MySqlCommand(command, conn.Connection);
                m.Parameters.AddWithValue("@param_val_1", Base);
                m.Parameters.AddWithValue("@param_val_2", userId);
                await m.ExecuteNonQueryAsync();
            }
            await conn.Close();
        }

        public static async Task SetMessage(uint userId, string message)
        {
            message = Utilities.Encoding.RemoveSpecialCharacters(message);

            var conn = new OpenConnection();
            if (conn.IsConnected())
            {
                const string command = "UPDATE friendsafari SET message = @param_val_1 WHERE user_id = @param_val_2";
                var m = new MySqlCommand(command, conn.Connection);
                m.Parameters.AddWithValue("@param_val_1", message);
                m.Parameters.AddWithValue("@param_val_2", userId);
                await m.ExecuteNonQueryAsync();
            }
            await conn.Close();
        }

        public static async Task RemoveMessage(uint userId)
        {
            var conn = new OpenConnection();
            if (conn.IsConnected())
            {
                const string command = "UPDATE friendsafari SET message = NULL WHERE user_id = @param_val_2";
                var m = new MySqlCommand(command, conn.Connection);
                m.Parameters.AddWithValue("@param_val_2", userId);
                await m.ExecuteNonQueryAsync();
            }
            await conn.Close();
        }
        /// <summary>
        /// Add a gift to a giftbox
        /// </summary>
        /// <returns>0 if something went wrong, 1 if gift can't be given, 2 if giftbox is full, 3 if success</returns>
        public static async Task<int> AddGift(uint gift, string username)
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
            var r = await m.ExecuteReaderAsync();

            uint userId = 0;
            var oldGifts = string.Empty;

            while (await r.ReadAsync())
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

            var currentGiftAmount = giftsList.Sum(x => x.Amount);
            if (currentGiftAmount >= Data.MaximumGifts)
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

            await im.ExecuteNonQueryAsync();

            return 3;
        }

        public static async Task GetGifts(Client client)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
                return;

            //Get the gifts
            const string command = "SELECT giftbox FROM friendsafari WHERE user_id = @param_val_1";
            var m = new MySqlCommand(command, conn.Connection);
            m.Parameters.AddWithValue("@param_val_1", client.UserId);
            var gifts = await m.ExecuteScalarAsync();

            string giftString;
            if (gifts == null || gifts is DBNull)
            {
                giftString = "nil";
            }
            else
            {
                giftString = gifts.ToString();
            }

            //Send the gifts
            await client.SendMessage($"<FSGIFTS gifts={giftString}>");

            //Remove the gifts from the database
            const string removeCommand = "UPDATE friendsafari SET giftbox = NULL WHERE user_id = @param_val_1";
            var rm = new MySqlCommand(removeCommand, conn.Connection);
            rm.Parameters.AddWithValue("@param_val_1", client.UserId);
            await rm.ExecuteNonQueryAsync();
        }

        public static async Task SetTrainer(Client client, string trainer)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
                return;

            const string command = "UPDATE friendsafari SET trainer = @param_val_1 WHERE user_id = @param_val_2";
            var m = new MySqlCommand(command, conn.Connection);
            m.Parameters.AddWithValue("@param_val_1", trainer);
            m.Parameters.AddWithValue("@param_val_2", client.UserId);
            await m.ExecuteNonQueryAsync();

            await conn.Close();
        }

        public static async Task GetTrainer(Client client, string username)
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
                return;

            const string command = "SELECT trainer " +
                                   "FROM friendsafari " +
                                   "INNER JOIN users " +
                                   "ON users.user_id = friendsafari.user_id " +
                                   "WHERE users.username = @param_val_1";
            var m = new MySqlCommand(command, conn.Connection);
            m.Parameters.AddWithValue("@param_val_1", username);

            var trainerObj = await m.ExecuteScalarAsync();
            if (trainerObj is DBNull)
            {
                await client.SendMessage($"<BASETRA result=0 trainer=nil>");
            }
            else
            {
                await client.SendMessage($"<BASETRA result=1 trainer={Utilities.Encoding.Base64Encode(trainerObj.ToString())}>");
            }
        }


        private static bool VerifyGift(uint gift)
        {
            //TODO actually write this
            return true;
        }

        private class GiftHolder
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
