using System.Linq;
using System.Threading.Tasks;
using InsurgenceServer.Database;
using InsurgenceServer.GTS;

namespace InsurgenceServer.Trades
{
    public static class TradeValidator
    {
        public static async Task<bool> IsPokemonValid(GamePokemon pokemon, uint userId)
        {
            if (Data.BannedOTs.Contains(pokemon.ot.ToLowerInvariant()))
            {
                await DBWarnLog.LogWarning(userId, $"Trading pokemon with Banned OT: {pokemon.ot}");
                return false;
            }
            if (Data.BannedTrainerIDs.Contains(pokemon.trainerID))
            {
                await DBWarnLog.LogWarning(userId, $"Trading pokemon with Banned Trainer ID: {pokemon.trainerID}");
                return false;
            }
            if (pokemon.iv.Any(x => x > 31))
            {
                await DBWarnLog.LogWarning(userId,
                    $"Trading pokemon with IV higher than 31: {pokemon.iv.FirstOrDefault(x => x > 31)}");
                return false;
            }
            if (pokemon.ev.Any(x => x > 255))
            {
                await DBWarnLog.LogWarning(userId,
                    $"Trading pokemon with EV higher than 255: {pokemon.ev.FirstOrDefault(x => x > 255)}");
                return false;
            }
            if (pokemon.ev.Sum() > 510)
            {
                await DBWarnLog.LogWarning(userId, $"Trading pokemon with total EV higher than 510: {pokemon.ev.Sum()}");
                return false;
            }
            if (!AllowedObtainTexts.Contains(pokemon.obtainText.ToLowerInvariant()))
            {
                await DBWarnLog.LogWarning(userId, $"Trading pokemon with invalid obtain text: {pokemon.obtainText}");
                return false;
            }
            if (pokemon.ot.Length > 20)
            {
                await DBWarnLog.LogWarning(userId, $"Trading pokemon with too long OT: {pokemon.ot}");
                return false;
            }
            if (pokemon.name != null && pokemon.name.Length > 20)
            {
                await DBWarnLog.LogWarning(userId, $"Trading pokemon with too long name: {pokemon.name}");
                return false;
            }

            return true;
        }

        public static readonly string[] AllowedObtainTexts = new[]
        {
            "", "day-care couple", "faraway place", "mystery gift"
        };
    }
}