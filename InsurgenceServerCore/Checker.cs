using System;
using System.Threading.Tasks;
using InsurgenceServerCore.Battles;
using InsurgenceServerCore.Trades;

namespace InsurgenceServerCore
{
    public static class Checker
    {
        public static async Task BeginChecking()
        {
            while (Data.Running)
            {
                try
                {
                    await ClientHandler.ClientHandler.ClientChecker();
                }
                catch (Exception e)
                {
                    Logger.ErrorLog.Log(e);
                }
                try
                {
                    await TradeHandler.TradeChecker();
                }
                catch (Exception e)
                {
                    Logger.ErrorLog.Log(e);
                }
                try
                {
                    await BattleHandler.BattleChecker();
                }
                catch (Exception e)
                {
                    Logger.ErrorLog.Log(e);
                }
                try
                {
                    await BattleHandler.BattleChecker();
                }
                catch (Exception e)
                {
                    Logger.ErrorLog.Log(e);
                }

                await Task.Delay(2000);
            }
        }
    }
}