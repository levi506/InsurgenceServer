using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace InsurgenceServer.Battles
{
    public static class RandomBattles
    {
        private static readonly Dictionary<Client, DateTime> NoTierRandoms = new Dictionary<Client, DateTime>();
        private static readonly Dictionary<Client, DateTime> AgRandoms = new Dictionary<Client, DateTime>();
        private static readonly Dictionary<Client, DateTime> UberRandoms = new Dictionary<Client, DateTime>();
        private static readonly Dictionary<Client, DateTime> OuRandoms = new Dictionary<Client, DateTime>();
        private static readonly Dictionary<Client, DateTime> BlRandoms = new Dictionary<Client, DateTime>();
        private static readonly Dictionary<Client, DateTime> UuRandoms = new Dictionary<Client, DateTime>();
        private static readonly Dictionary<Client, DateTime> RuRandoms = new Dictionary<Client, DateTime>();
        private static readonly Dictionary<Client, DateTime> NuRandoms = new Dictionary<Client, DateTime>();

        public static void AddRandom(Client c, Tiers tier, Tiers maxTier)
        {
            if (tier == Tiers.Notier)
            {
                c.QueuedTier = Tiers.Notier;
                NoTierRandoms.Add(c, DateTime.Now);
            }
            else if ((int)tier > (int)maxTier)
            {
                c.SendMessage("<RAND INC>");
            }
            else
            {
                if (tier == Tiers.Ag)
                    AgRandoms.Add(c, DateTime.Now);
                else if (tier == Tiers.Uber)
                    UberRandoms.Add(c, DateTime.Now);
                else if (tier == Tiers.Ou)
                    OuRandoms.Add(c, DateTime.Now);
                else if (tier == Tiers.Bl)
                    BlRandoms.Add(c, DateTime.Now);
                else if (tier == Tiers.Uu)
                    UuRandoms.Add(c, DateTime.Now);
                else if (tier == Tiers.Ru)
                    RuRandoms.Add(c, DateTime.Now);
                else if (tier == Tiers.Nu)
                    NuRandoms.Add(c, DateTime.Now);
            }
        }
        public static void RemoveRandom(Client c)
        {
            var tier = c.QueuedTier;
            if (tier == Tiers.Ag)
                AgRandoms.Remove(c);
            else if (tier == Tiers.Uber)
                UberRandoms.Remove(c);
            else if (tier == Tiers.Ou)
                OuRandoms.Remove(c);
            else if (tier == Tiers.Bl)
                BlRandoms.Remove(c);
            else if (tier == Tiers.Uu)
                UuRandoms.Remove(c);
            else if (tier == Tiers.Ru)
                RuRandoms.Remove(c);
            else if (tier == Tiers.Nu)
                NuRandoms.Remove(c);
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
                    else if (AgRandoms.Count >= 2)
                    {
                        MatchUsers(AgRandoms);
                    }
                    else if (UberRandoms.Count >= 2)
                    {
                        MatchUsers(UberRandoms);
                    }
                    else if (OuRandoms.Count >= 2)
                    {
                        MatchUsers(OuRandoms);
                    }
                    else if (BlRandoms.Count >= 2)
                    {
                        MatchUsers(BlRandoms);
                    }
                    else if (UuRandoms.Count >= 2)
                    {
                        MatchUsers(UuRandoms);
                    }
                    else if (RuRandoms.Count >= 2)
                    {
                        MatchUsers(RuRandoms);
                    }
                    else if (NuRandoms.Count >= 2)
                    {
                        MatchUsers(NuRandoms);
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(2000);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.ErrorLog.Log(e);
                MatchRandoms();
            }
        }
        public static void CleanRandoms()
        {
            while (true)
            {
                foreach(var kp in AgRandoms)
                {
                    if ((kp.Value - DateTime.Now).TotalSeconds > 60) AgRandoms.Remove(kp.Key);
                }
                foreach (var kp in UberRandoms)
                {
                    if ((kp.Value - DateTime.Now).TotalSeconds > 60) AgRandoms.Remove(kp.Key);
                }
                foreach (var kp in OuRandoms)
                {
                    if ((kp.Value - DateTime.Now).TotalSeconds > 60) AgRandoms.Remove(kp.Key);
                }
                foreach (var kp in BlRandoms)
                {
                    if ((kp.Value - DateTime.Now).TotalSeconds > 60) AgRandoms.Remove(kp.Key);
                }
                foreach (var kp in UuRandoms)
                {
                    if ((kp.Value - DateTime.Now).TotalSeconds > 60) AgRandoms.Remove(kp.Key);
                }
                foreach (var kp in RuRandoms)
                {
                    if ((kp.Value - DateTime.Now).TotalSeconds > 60) AgRandoms.Remove(kp.Key);
                }
                foreach (var kp in NuRandoms)
                {
                    if ((kp.Value - DateTime.Now).TotalSeconds > 60) AgRandoms.Remove(kp.Key);
                }
                System.Threading.Thread.Sleep(5000);
            }
        }
        public static void MatchUsers(Dictionary<Client,DateTime> l)
        {
            var r = new Random();
            var u1 = l.Keys.ElementAt(r.Next(0, l.Count));
            var u2 = l.Keys.ElementAt(r.Next(0, l.Count));
            if (u1 != u2 && u1.Username != u2.Username)
            {
                u1.SendMessage($"<RANDBAT user={u2.Username}>");
                u2.SendMessage($"<RANDBAT user={u1.Username}>");
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
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    Tiers = JsonConvert.DeserializeObject<Tierlist>(result);
                }
            }
        }
        public static Tiers GetTier(string speciesList)
        {
            var a = speciesList.Split('/');
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
            if (Tiers.Ag.Contains(spefor, itemarr))
                return Battles.Tiers.Ag;
            else if (Tiers.Uber.Contains(spefor, itemarr))
                return Battles.Tiers.Uber;
            else if (Tiers.Ou.Contains(spefor, itemarr))
                return Battles.Tiers.Ou;
            else if (Tiers.Bl.Contains(spefor, itemarr))
                return Battles.Tiers.Bl;
            else if (Tiers.Uu.Contains(spefor, itemarr))
                return Battles.Tiers.Uu;
            else if (Tiers.Ru.Contains(spefor, itemarr))
                return Battles.Tiers.Ru;
            else
                return Battles.Tiers.Nu;
        }
    }
    public enum Tiers
    {
        Null = 0, Notier, Ag, Uber, Ou, Bl, Uu, Ru, Nu
    }
    public class Tierlist
    {
        public Tier Ag;
        public Tier Uber;
        public Tier Ou;
        public Tier Bl;
        public Tier Uu;
        public Tier Ru;
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
