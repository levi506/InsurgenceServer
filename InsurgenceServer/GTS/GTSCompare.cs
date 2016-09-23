namespace InsurgenceServer.GTS
{
    public static class GtsCompare
    {
        public static bool ValidOffer(GamePokemon pokemon, RequestData request)
        {
            var pokelevel = GrowthRates.CalculateLevel(pokemon.Species ,pokemon.Exp);
            if (pokelevel < request.MinLevel)
                return false;
            return true;
        }
    }
}
