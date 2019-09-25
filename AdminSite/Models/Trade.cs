using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace AdminSite.Models
{
    public class Trade
    {
        public DateTime Date { get; set; }
        public string User1 { get; set; }
        public string User2 { get; set; }
        public Pokemon Pokemon1 { get; set; }
        public Pokemon Pokemon2 { get; set; }
        public int Id { get; set; }
    }
    public class WonderTrade
    {
        public uint Id { get; set; }
        public DateTime Date { get; set; }
        public string User { get; set; }
        public Pokemon Pokemon { get; set; }
    }
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Pokemon
    {
        public byte ability { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public byte abilityflag { get; set; }
        public int attack { get; set; }
        
        public bool ballcapsule0 { get; set; }
        public bool ballcapsule1 { get; set; }
        public bool ballcapsule2 { get; set; }
        public bool ballcapsule3 { get; set; }
        public bool ballcapsule4 { get; set; }
        public bool ballcapsule5 { get; set; }
        public bool ballcapsule6 { get; set; }
        public bool ballcapsule7 { get; set; }

        public int ballused { get; set; }
        public bool belch { get; set; }
        public bool burstAttack { get; set; }
        public int defense { get; set; }
        public GameMove[] eggmovesarray { get; set; }
        public int eggsteps { get; set; }
        public int[] ev { get; set; } = new int[6];
        public int exp { get; set; }
        public byte form { get; set; }
        public string fused { get; set; }
        public byte gender { get; set; }
        public int happiness { get; set; }
        public int hatchedMap { get; set; }
        public int heartgauge { get; set; }
        public int hp { get; set; }
        public bool hypermode { get; set; }
        public bool isShiny { get; set; }
        public int item { get; set; }
        public int itemInitial { get; set; }
        public int itemRecycle { get; set; }
        public int[] iv { get; set; } = new int[6];
        public byte language { get; set; }
        public byte level { get; set; }
        public string mail { get; set; }
        public int? markings { get; set; }
        public bool megaFlygon { get; set; }
        public bool megaTyranitar { get; set; }
        public GameMove[] moves { get; set; } = {new GameMove(), new GameMove(), new GameMove(), new GameMove()};
        public string name { get; set; }
        public byte nature { get; set; }
        public bool normalMegaMewtwoX { get; set; }
        public bool normalMegaMewtwoY { get; set; }
        public bool normalMewtwo { get; set; }
        public int obtainLevel { get; set; }
        public int obtainMap { get; set; }
        public int obtainMode { get; set; }
        public string obtainText { get; set; }
        public string ot { get; set; }
        public byte otgender { get; set; }
        public uint personalID { get; set; }
        public bool primalBattle { get; set; }
        public int[] ribbons { get; set; }
        public int spatk { get; set; }
        public int spdef { get; set; }
        public short species { get; set; }
        public int speed { get; set; }
        public int status { get; set; }
        public int statusCount { get; set; }
        public long timeReceived { get; set; }
        public int totalhp { get; set; }
        public uint trainerID { get; set; }
        public int zygardeForm { get; set; }
    }
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GameMove
    {
        public int ppup { get; set; }
        public int id { get; set; }
        public int pp { get; set; }
    }

    public enum PokemonGender
    {
        Male,
        Female,
        Genderless
    }

    public enum Nature
    {
        Hardy,
        Lonely,
        Brave,
        Adamant,
        Naughty,
        Bold,
        Docile,
        Relaxed,
        Impish,
        Lax,
        Timid,
        Hasty,
        Serious,
        Jolly,
        Naive,
        Modest,
        Mild,
        Quiet,
        Bashful,
        Rash,
        Calm,
        Gentle,
        Sassy,
        Careful,
        Quirky
    }
}
