using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsurgenceServerWebsite.Models
{
    public class RecentTradesModel
    {
        public uint StartIndex { get; set; }
        public List<Trade> Trades { get; set; }
    }
}
