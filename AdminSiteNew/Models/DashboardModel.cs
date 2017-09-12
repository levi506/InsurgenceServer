using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSiteNew.Models
{
    public class DashboardModel
    {
        public int UserCount => ServerInteraction.Handler.UserCount;

        public int TradeCount => ServerInteraction.Handler.TradeCount;

        public int BattleCount => ServerInteraction.Handler.BattleCount;

        public int WTCount => ServerInteraction.Handler.WTCount;
    }
}
