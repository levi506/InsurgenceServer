using System.Diagnostics.CodeAnalysis;
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace InsurgenceServerCore.GTS
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GamePokemon
    {
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
        public int[] ev { get; set; }
        public int exp { get; set; }
        public byte form { get; set; }
        public string fused { get; set; }
        public byte gender { get; set; }
        public byte happiness { get; set; }
        public int hatchedMap { get; set; }
        public int heartgauge { get; set; }
        public int hp { get; set; }
        public bool hypermode { get; set; }
        public bool isShiny { get; set; }
        public int item { get; set; }
        public int itemInitial { get; set; }
        public int itemRecycle { get; set; }
        public int[] iv { get; set; }
        public byte language { get; set; }
        public byte level { get; set; }
        public string mail { get; set; }
        public int? markings { get; set; }
        public bool megaFlygon { get; set; }
        public bool megaTyranitar { get; set; }
        public GameMove[] moves { get; set; }
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
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GameMove
    {
        public int ppup { get; set; }
        public int id { get; set; }
        public int pp { get; set; }
    }
}
