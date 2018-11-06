using System;
using System.Linq;
using System.Threading.Tasks;
using InsurgenceServerCore.Database;
using InsurgenceServerCore.GTS;

namespace InsurgenceServerCore.Trades
{
    public static class TradeValidator
    {
        public static async Task<bool> IsPokemonValid(GamePokemon pokemon, uint userId)
        {
            if (Data.BannedOTs.Any(x => string.Equals(x, pokemon.ot, StringComparison.InvariantCultureIgnoreCase)))
            {
                await DBWarnLog.LogWarning(userId, $"Trading pokemon with Banned OT: {pokemon.ot}");
                return false;
            }
            if (Data.BannedTrainerIDs.Contains(pokemon.trainerID))
            {
                await DBWarnLog.LogWarning(userId, $"Trading pokemon with Banned Trainer ID: {pokemon.trainerID}");
                return false;
            }
            if (pokemon.iv.Any(x => x > 31 || x < 0))
            {
                await DBWarnLog.LogWarning(userId,
                    $"Trading pokemon with IV higher than 31/lower than 0: {pokemon.iv.FirstOrDefault(x => x > 31)}");
                return false;
            }
            if (pokemon.ev.Any(x => x > 255 || x < 0))
            {
                await DBWarnLog.LogWarning(userId,
                    $"Trading pokemon with EV higher than 255/lower than 0: {pokemon.ev.FirstOrDefault(x => x > 255)}");
                return false;
            }
            var evSum = pokemon.ev.Sum();
            if (evSum > 510 || evSum < 0)
            {
                await DBWarnLog.LogWarning(userId, $"Trading pokemon with total EV higher than 510/lower than 0: {pokemon.ev.Sum()}");
                return false;
            }
            if (!AllowedObtainTexts.Any(x => string.Equals(x, pokemon.obtainText, StringComparison.InvariantCultureIgnoreCase)))
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

        private static readonly string[] AllowedObtainTexts = {
            "", "day-care couple", "faraway place", "mystery gift", "santa's workshop", "halloween 2018"
        };
    }
}