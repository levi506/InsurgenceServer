using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InsurgenceServer.Battles
{
    public static class RandomBattles
    {
        private static List<Client> NoTierRandoms = new List<Client>();
        private static List<Client> AGRandoms = new List<Client>();
        private static List<Client> UberRandoms = new List<Client>();
        private static List<Client> OURandoms = new List<Client>();
        private static List<Client> BLRandoms = new List<Client>();
        private static List<Client> UURandoms = new List<Client>();
        private static List<Client> RURandoms = new List<Client>();
        private static List<Client> NURandoms = new List<Client>();

        public static void AddRandom(Client c, Tiers tier, Tiers maxTier)
        {
            if (tier == Tiers.notier)
            {
                c.QueuedTier = Tiers.notier;
                NoTierRandoms.Add(c);
            }
            else if ((int)tier > (int)maxTier)
            {
                c.SendMessage("<RAND INC>");
            }
            else
            {
                if (tier == Tiers.AG)
                    AGRandoms.Add(c);
                else if (tier == Tiers.Uber)
                    UberRandoms.Add(c);
                else if (tier == Tiers.OU)
                    OURandoms.Add(c);
                else if (tier == Tiers.BL)
                    BLRandoms.Add(c);
                else if (tier == Tiers.UU)
                    UURandoms.Add(c);
                else if (tier == Tiers.RU)
                    RURandoms.Add(c);
                else if (tier == Tiers.NU)
                    NURandoms.Add(c);
            }
        }
        public static void RemoveRandom(Client c)
        {
            var tier = c.QueuedTier;
            if (tier == Tiers.AG)
                AGRandoms.RemoveAll(x => x == c);
            else if (tier == Tiers.Uber)
                UberRandoms.RemoveAll(x => x == c);
            else if (tier == Tiers.OU)
                OURandoms.RemoveAll(x => x == c);
            else if (tier == Tiers.BL)
                BLRandoms.RemoveAll(x => x == c);
            else if (tier == Tiers.UU)
                UURandoms.RemoveAll(x => x == c);
            else if (tier == Tiers.RU)
                RURandoms.RemoveAll(x => x == c);
            else if (tier == Tiers.NU)
                NURandoms.RemoveAll(x => x == c);
        }
        public static void MatchRandoms()
        {
            try
            {
                while (Data.Running)
                {
                    if (NoTierRandoms.Count >= 2)
                    {
                        MatchUsers(NoTierRandoms);
                    }
                    else if (AGRandoms.Count >= 2)
                    {
                        MatchUsers(AGRandoms);
                    }
                    else if (UberRandoms.Count >= 2)
                    {
                        MatchUsers(UberRandoms);
                    }
                    else if (OURandoms.Count >= 2)
                    {
                        MatchUsers(OURandoms);
                    }
                    else if (BLRandoms.Count >= 2)
                    {
                        MatchUsers(BLRandoms);
                    }
                    else if (UURandoms.Count >= 2)
                    {
                        MatchUsers(UURandoms);
                    }
                    else if (RURandoms.Count >= 2)
                    {
                        MatchUsers(RURandoms);
                    }
                    else if (NURandoms.Count >= 2)
                    {
                        MatchUsers(NURandoms);
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(2000);
                    }
                }
            }
            catch
            {
                MatchRandoms();
            }
        }
        public static void MatchUsers(List<Client> l)
        {
            var r = new Random();
            var u1 = l[r.Next(0, l.Count)];
            var u2 = l[r.Next(0, l.Count)];
            if (u1 != u2 && u1.Username != u2.Username)
            {
                u1.SendMessage(string.Format("<RANDBAT user={0}>", u2.Username));
                u2.SendMessage(string.Format("<RANDBAT user={0}>", u1.Username));
                l.Remove(u1);
                l.Remove(u2);
            }
        }
    }
    public static class Matchmaking
    {
        public static Tierlist Tiers;
        public static void SetupTiers()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "InsurgenceServer.Battles.Tiers.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                Tiers = JsonConvert.DeserializeObject<Tierlist>(result);
            }
        }
        public static Tiers GetTier(string SpeciesList)
        {
            var a = SpeciesList.Split('/');
            var speciestr = a[0];
            var itemstr = a[1];
            List<List<int>> spefor = new List<List<int>>();
            var speciearr = speciestr.Split('^');
            foreach (var s in speciearr)
            {
                var t = s.Split('_');
                List<int> thing = new List<int>();
                foreach (var bb in t)
                {
                    thing.Add(int.Parse(bb));
                }
                spefor.Add(thing);
            }
            var itemarr = itemstr.Split('^');
            //Check AG
            if (Tiers.AG.Contains(spefor, itemarr))
                return Battles.Tiers.AG;
            else if (Tiers.Uber.Contains(spefor, itemarr))
                return Battles.Tiers.Uber;
            else if (Tiers.OU.Contains(spefor, itemarr))
                return Battles.Tiers.OU;
            else if (Tiers.BL.Contains(spefor, itemarr))
                return Battles.Tiers.BL;
            else if (Tiers.UU.Contains(spefor, itemarr))
                return Battles.Tiers.UU;
            else if (Tiers.RU.Contains(spefor, itemarr))
                return Battles.Tiers.RU;
            else
                return Battles.Tiers.NU;
        }
    }
    public enum Tiers
    {
        Null = 0, notier, AG, Uber, OU, BL, UU, RU, NU
    }
    public class Tierlist
    {
        public Tier AG;
        public Tier Uber;
        public Tier OU;
        public Tier BL;
        public Tier UU;
        public Tier RU;
    }
    public class Tier
    {
        public string Tiername;
        public List<int> Species;
        public List<int> Items;
        public List<List<int>> Formes;

        public bool Contains(List<List<int>> speciesformes, string[] items)
        {
            foreach (var s in speciesformes)
            {
                if (Species.Contains(s[0]))
                    return true;
                if (Formes.Contains(s))
                    return true;
            }
            foreach (var i in items)
            {
                if (Items.Contains(int.Parse(i)))
                    return true;
            }
            return false;
        }
    }
}
