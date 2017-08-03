namespace InsurgenceServer.GTS
{
    public static class GtsCompare
    {
        public static bool ValidOffer(GamePokemon pokemon, RequestData request)
        {
            var pokelevel = GrowthRates.CalculateLevel(pokemon.species ,pokemon.exp);
            if (pokelevel < request.MinLevel)
            {
                return false;
            }
            
            if (request.Nature != 25 && request.Nature != pokemon.nature)
            {
                return false;
            }

            if (request.Species != 0 && request.Species != pokemon.species)
            {
                return false;
            }

            if (request.Gender != 2 && request.Gender != pokemon.gender)
            {
                return false;
            }
            return true;
        }
    }
}
