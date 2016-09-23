namespace InsurgenceServer.GTS
{
    public static class GtsCompare
    {
        public static bool ValidOffer(GamePokemon pokemon, RequestData request)
        {
            var pokelevel = GrowthRates.CalculateLevel(pokemon.Species ,pokemon.Exp);
            return pokelevel >= request.MinLevel;
        }
    }
}
