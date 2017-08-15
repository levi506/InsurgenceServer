using System;

namespace InsurgenceServer.GTS
{
    public static class GtsCompare
    {
        public static bool ValidOffer(GamePokemon pokemon, RequestData request)
        {
            if (pokemon.level < request.MinLevel)
            {
                Logger.Logger.Log("GTS trade rejected on level");
                return false;
            }
            
            if (request.Nature != 25 && request.Nature != pokemon.nature)
            {
                Logger.Logger.Log("GTS trade rejected on nature");
                return false;
            }

            if (request.Species != 0 && request.Species != pokemon.species)
            {
                Logger.Logger.Log("GTS trade rejected on species");
                return false;
            }

            if (request.Gender != 2 && request.Gender != pokemon.gender)
            {
                Logger.Logger.Log("GTS trade rejected on gender");
                return false;
            }
            return true;
        }
    }
}
