using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSiteNew.Models
{
    public class DashboardModel
    {
        public int UserCount
        {
            get
            {
                return ServerInteraction.Handler.UserCount;
            }
            private set
            {
                ServerInteraction.Handler.UserCount = value;
            }
        }
        public int TradeCount
        {
            get
            {
                return ServerInteraction.Handler.TradeCount;
            }
            private set
            {
                ServerInteraction.Handler.TradeCount = value;
            }
        }
        public int BattleCount
        {
            get
            {
                return ServerInteraction.Handler.BattleCount;
            }
            private set
            {
                ServerInteraction.Handler.BattleCount = value;
            }
        }
        public int WTCount
        {
            get
            {
                return ServerInteraction.Handler.WTCount;
            }
            private set
            {
                ServerInteraction.Handler.WTCount = value;
            }
        }
    }
}
