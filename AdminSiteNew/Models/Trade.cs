using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSiteNew.Models
{
    public class Trade
    {
        public DateTime Date { get; set; }
        public string User1 { get; set; }
        public string User2 { get; set; }
        public Pokemon Pokemon1 { get; set; }
        public Pokemon Pokemon2 { get; set; }
    }
    public class Pokemon
    {
        public short Species;
        public string TrainerId;
        public string ID;
        public bool Shiny;
        public short Item;
        public string OT;
    }
}
