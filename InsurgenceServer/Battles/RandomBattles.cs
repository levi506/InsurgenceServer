using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using InsurgenceServer.ClientHandler;

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

        private const int QueueTimeout = 60;


        public static async Task AddRandom(Client c, Tiers tier, Tiers maxTier)
        {
            if (tier == Tiers.Notier)
            {
                c.QueuedTier = Tiers.Notier;
                NoTierRandoms.Add(c, DateTime.Now);
            }
            else if ((int)tier > (int)maxTier)
            {
                await c.SendMessage("<RAND INC>");
            }
            else
            {
                switch (tier)
                {
                    case Tiers.Ag:
                        AgRandoms.Add(c, DateTime.Now);
                        break;
                    case Tiers.Uber:
                        UberRandoms.Add(c, DateTime.Now);
                        break;
                    case Tiers.Ou:
                        OuRandoms.Add(c, DateTime.Now);
                        break;
                    case Tiers.Bl:
                        BlRandoms.Add(c, DateTime.Now);
                        break;
                    case Tiers.Uu:
                        UuRandoms.Add(c, DateTime.Now);
                        break;
                    case Tiers.Ru:
                        RuRandoms.Add(c, DateTime.Now);
                        break;
                    case Tiers.Nu:
                        NuRandoms.Add(c, DateTime.Now);
                        break;
                }
            }
        }
        public static async Task RemoveRandom(Client c)
        {
            var tier = c.QueuedTier;
            switch (tier)
            {
                case Tiers.Ag:
                    AgRandoms.Remove(c);
                    break;
                case Tiers.Uber:
                    UberRandoms.Remove(c);
                    break;
                case Tiers.Ou:
                    OuRandoms.Remove(c);
                    break;
                case Tiers.Bl:
                    BlRandoms.Remove(c);
                    break;
                case Tiers.Uu:
                    UuRandoms.Remove(c);
                    break;
                case Tiers.Ru:
                    RuRandoms.Remove(c);
                    break;
                case Tiers.Nu:
                    NuRandoms.Remove(c);
                    break;
            }
        }
        public static async Task MatchRandoms()
        {
            try
            {
                while (Data.Running)
                {
                    if (NoTierRandoms.Count >= 2)
                    {
                        await MatchUsers(NoTierRandoms);
                    }
                    else if (AgRandoms.Count >= 2)
                    {
                        await MatchUsers(AgRandoms);
                    }
                    else if (UberRandoms.Count >= 2)
                    {
                        await MatchUsers(UberRandoms);
                    }
                    else if (OuRandoms.Count >= 2)
                    {
                        await MatchUsers(OuRandoms);
                    }
                    else if (BlRandoms.Count >= 2)
                    {
                        await MatchUsers(BlRandoms);
                    }
                    else if (UuRandoms.Count >= 2)
                    {
                        await MatchUsers(UuRandoms);
                    }
                    else if (RuRandoms.Count >= 2)
                    {
                        await MatchUsers(RuRandoms);
                    }
                    else if (NuRandoms.Count >= 2)
                    {
                        await MatchUsers(NuRandoms);
                    }
                    else
                    {
                        await Task.Delay(5000);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.ErrorLog.Log(e);
                await MatchRandoms();
            }
        }
        public static async Task CleanRandoms()
        {
            foreach(var kp in AgRandoms)
            {
                if ((kp.Value - DateTime.Now).TotalSeconds > QueueTimeout) AgRandoms.Remove(kp.Key);
            }
            foreach (var kp in UberRandoms)
            {
                if ((kp.Value - DateTime.Now).TotalSeconds > QueueTimeout) AgRandoms.Remove(kp.Key);
            }
            foreach (var kp in OuRandoms)
            {
                if ((kp.Value - DateTime.Now).TotalSeconds > QueueTimeout) AgRandoms.Remove(kp.Key);
            }
            foreach (var kp in BlRandoms)
            {
                if ((kp.Value - DateTime.Now).TotalSeconds > QueueTimeout) AgRandoms.Remove(kp.Key);
            }
            foreach (var kp in UuRandoms)
            {
                if ((kp.Value - DateTime.Now).TotalSeconds > QueueTimeout) AgRandoms.Remove(kp.Key);
            }
            foreach (var kp in RuRandoms)
            {
                if ((kp.Value - DateTime.Now).TotalSeconds > QueueTimeout) AgRandoms.Remove(kp.Key);
            }
            foreach (var kp in NuRandoms)
            {
                if ((kp.Value - DateTime.Now).TotalSeconds > QueueTimeout) AgRandoms.Remove(kp.Key);
            }

        }
        public static async Task MatchUsers(Dictionary<Client,DateTime> l)
        {
            var r = new Random();
            var u1 = l.Keys.ElementAt(r.Next(0, l.Count));
            var u2 = l.Keys.ElementAt(r.Next(0, l.Count));
            if (u1 != u2 && u1.Username != u2.Username)
            {
                await u1.SendMessage($"<RANDBAT user={u2.Username}>");
                await u2.SendMessage($"<RANDBAT user={u1.Username}>");
                l.Remove(u1);
                l.Remove(u2);
            }
        }
    }
    public static class Matchmaking
    {
        public static Tierlist Tiers;
        public static async Task SetupTiers()
        {
            var assembly = Assembly.GetExecutingAssembly();
            const string resourceName = "InsurgenceServer.Battles.Tiers.json";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) throw new NullReferenceException("Null tier file");
                using (var reader = new StreamReader(stream))
                {
                    var result = await reader.ReadToEndAsync();
                    Tiers = JsonConvert.DeserializeObject<Tierlist>(result);
                }
            }
        }
        public static Tiers GetTier(string speciesList)
        {
            var a = speciesList.Split('/');
            var speciestr = a[0];
            var itemstr = a[1];
            var speciearr = speciestr.Split('^');
            var spefor = speciearr.Select(s => s.Split('_')).Select(t => t.Select(int.Parse).ToList()).ToList();
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
            return items.Any(i => Items.Contains(int.Parse(i)));
        }
    }
}
