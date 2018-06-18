using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminSiteNew.Pokemon;

namespace AdminSiteNew.Models
{
    public class UserRequest
    {
        public UserInfo UserInfo { get; set; }
        public string FriendSafariString { get; set; }
        public FriendSafari FriendSafari { get; set; }
        public List<IPInfo> IPs { get; set; }
        public List<UserInfo> Alts { get; set; }
        public List<Trade> Trades {get;set;}
        public List<WonderTrade> WonderTrades { get; set; }
        public List<WarningsModel> Warnings { get; set; }
        public List<NotesModel> Notes { get; set; }
        public List<GTSObject> GTS { get; set; }
    }
    public class UserInfo
    {
        public string Username { get; set; }
        public DateTime LastLoggedIn { get; set; }
        public uint User_Id { get; set; }
        public bool Banned {get;set;}
    }
    public class IPInfo
    {
        public string IP{get;set;}
        public bool Banned{get;set;}
    }

    public class NotesModel
    {
        public string Moderator { get; set; }
        public string Note { get; set; }
        public DateTime Time { get; set; }
    }

    public class FriendSafari
    {
        public FriendSafariType Type { get; set; } = FriendSafariType.None;
        public List<PokemonList> Pokemon { get; set; }

        public FriendSafari(string s)
        {
            if (string.IsNullOrEmpty(s)) return;

            var basestring = s.Split('g');

            var pkmn = basestring[2].Split('f');
            Type = (FriendSafariType)int.Parse(pkmn[0]);

            Pokemon = new List<PokemonList>();
            for (var i = 1; i < pkmn.Length;i++)
            {
                var mon = pkmn[i];
                if (mon == "") continue;
                Pokemon.Add((PokemonList)(short.Parse(mon)));
            }
        }

        public string GetCleanPokemon => Pokemon == null ? "None" : string.Join(", ", Pokemon.ToArray());
    }
    public enum FriendSafariType
    {
        None = -1,
        Normal = 0, Fighting, Flying, Poison, Ground, Rock, Bug, Ghost, Steel, Missing, Fire, Water, Grass, Electric,
        Psychic, Ice, Dragon, Dark, Shadow, Bird, Weak, Fairy
    }
}
