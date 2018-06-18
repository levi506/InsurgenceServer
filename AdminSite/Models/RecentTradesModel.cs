using System.Collections.Generic;

namespace AdminSite.Models
{
    public class RecentTradesModel
    {
        public uint StartIndex { get; set; }
        public List<Trade> Trades { get; set; }
    }

    public class RecentWonderTradesModel
    {
        public uint StartIndex { get; set; }
        public List<WonderTrade> Trades { get; set; }
    }
}
