using System.Collections.Generic;

namespace InsurgenceServer.GTS
{
    public class GamePokemon
    {
        public int ItemInitial { get; set; }
        public bool Ballcapsule2 { get; set; }
        public int Obtainlevel { get; set; }
        public int Language { get; set; }
        public bool Hypermode { get; set; }
        public string Name { get; set; }
        public string Fused { get; set; }
        public int[] Ev { get; set; }
        public uint PersonalId { get; set; }
        public bool Ballcapsule5 { get; set; }
        public int Species { get; set; }
        public int Happiness { get; set; }
        public string Mail { get; set; }
        public bool Ballcapsule1 { get; set; }
        public int ItemRecycle { get; set; }
        public List<GameMove> Moves { get; set; }
        public int Ballused { get; set; }
        public int Hp { get; set; }
        public int Exp { get; set; }
        public int Spdef { get; set; }
        public int StatusCount { get; set; }
        public bool Ballcapsule4 { get; set; }
        public int Markings { get; set; }
        public int Attack { get; set; }
        public int Speed { get; set; }
        public int[] Iv { get; set; }
        public int ObtainMap { get; set; }
        public bool Ballcapsule0 { get; set; }
        public int HatchedMap { get; set; }
        public int Eggsteps { get; set; }
        public bool Ballcapsule7 { get; set; }
        public int Item { get; set; }
        public int Status { get; set; }
        public int Spatk { get; set; }
        public int Totalhp { get; set; }
        public uint TrainerId { get; set; }
        public int Otgender { get; set; }
        public bool Ballcapsule3 { get; set; }
        public int ObtainMode { get; set; }
        public string ObtainText { get; set; }
        public int Heartgauge { get; set; }
        public int Defense { get; set; }
        public bool Ballcapsule6 { get; set; }
        public string Ot { get; set; }
        public int TimeReceived { get; set; }
        public bool RibbonsAllowed { get; set; }
    }
    public class GameMove
    {
        public int Ppup { get; set; }
        public int Id { get; set; }
        public int Pp { get; set; }
    }
}
