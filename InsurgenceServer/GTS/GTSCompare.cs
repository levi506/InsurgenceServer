namespace InsurgenceServer.GTS
{
    public static class GtsCompare
    {
        public static bool ValidOffer(GamePokemon pokemon, RequestData request)
        {
            var pokelevel = GrowthRates.CalculateLevel(pokemon.species ,pokemon.exp);
            return pokelevel >= request.MinLevel;
        }
    }
}
