namespace AdminSite.Models
{
    public class DashboardModel
    {
        public int UserCount => ServerInteraction.Handler.UserCount;

        public int TradeCount => ServerInteraction.Handler.TradeCount;

        public int BattleCount => ServerInteraction.Handler.BattleCount;

        public int WTCount => ServerInteraction.Handler.WTCount;
    }
}
