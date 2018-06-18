using System;

namespace AdminSiteNew.Pokemon
{
    public static class Utilities
    {
        public static string GetPokemonImageName(int i, bool shiny)
        {
            var s = i.ToString();
            while (s.Length < 3)
            {
                s = "0" + s;
            }
           var pathname = shiny ? "/images/PokemonShinies/" : "/images/PokemonFronts/";

            return pathname + s + ".png";

        }

        public static string GetPokemonImageTitle(int i, bool shiny)
        {
            var s = "";
            if (shiny)
                s += "Shiny_";
            s += PokemonHelper.GetPokemonName((short) i);
            Console.WriteLine(s);
            return s;
        }
    }
}
