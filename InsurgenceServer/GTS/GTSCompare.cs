using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsurgenceServer.GTS
{
    public static class GTSCompare
    {
        public static bool ValidOffer(GamePokemon Pokemon, RequestData Request)
        {
            var pokelevel = GrowthRates.CalculateLevel(Pokemon.species ,Pokemon.exp);
            if (pokelevel < Request.MinLevel)
                return false;
            return true;
        }
    }
}
