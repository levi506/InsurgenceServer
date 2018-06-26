using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace InsurgenceServerCore.GTS
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GamePokemon
    {
        public int itemInitial { get; set; }
        public bool ballcapsule2 { get; set; }
        public int obtainlevel { get; set; }
        public int language { get; set; }
        public bool hypermode { get; set; }
        public string name { get; set; }
        public string fused { get; set; }
        public int[] ev { get; set; }
        public uint personalID { get; set; }
        public bool ballcapsule5 { get; set; }
        public short species { get; set; }
        public int happiness { get; set; }
        public string mail { get; set; }
        public bool ballcapsule1 { get; set; }
        public int itemRecycle { get; set; }
        public List<GameMove> moves { get; set; }
        public int ballused { get; set; }
        public int hp { get; set; }
        public int exp { get; set; }
        public int spdef { get; set; }
        public int statusCount { get; set; }
        public bool ballcapsule4 { get; set; }
        public int? markings { get; set; }
        public int attack { get; set; }
        public int speed { get; set; }
        public int[] iv { get; set; }
        public int obtainMap { get; set; }
        public bool ballcapsule0 { get; set; }
        public int hatchedMap { get; set; }
        public int eggsteps { get; set; }
        public bool ballcapsule7 { get; set; }
        public int item { get; set; }
        public int status { get; set; }
        public int spatk { get; set; }
        public int totalhp { get; set; }
        public uint trainerID { get; set; }
        public int otgender { get; set; }
        public bool ballcapsule3 { get; set; }
        public int obtainMode { get; set; }
        public string obtainText { get; set; }
        public int heartgauge { get; set; }
        public int defense { get; set; }
        public bool ballcapsule6 { get; set; }
        public string ot { get; set; }
        public int timeReceived { get; set; }


        public byte level { get; set; }
        public byte nature { get; set; }
        public bool isShiny { get; set; }
        public byte gender { get; set; }
    }
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GameMove
    {
        public int ppup { get; set; }
        public int id { get; set; }
        public int pp { get; set; }
    }
}
