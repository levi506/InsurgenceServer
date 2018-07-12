using System;
using System.Collections.Generic;
using InsurgenceServerCore.ClientHandler;

namespace InsurgenceServerCore.Utilities
{
    public static class FsChecker
    {
        public static bool IsValid(Client client, string Base)
        {
            if (client.Admin)
                return true;
            var basestring = Base.Split('g');

            var pkmn = basestring[2].Split('f');
            var type = (FriendSafariType)int.Parse(pkmn[0]);

            var pokemon = new List<PokemonList>();
            for (var i = 1; i < pkmn.Length; i++)
            {
                var mon = pkmn[i];
                if (mon == "") continue;
                pokemon.Add((PokemonList)Enum.Parse(typeof(PokemonList), mon));
            }
            for (var i = 0; i < 3; i++)
            {
                if (!LegalArray(type, i).Contains(pokemon[i]))
                {
#pragma warning disable 4014
                    Database.DBWarnLog.LogWarning(client.UserId,
                        "Tried to upload illegal Friend Safari. Contained: " + pokemon[i]);
#pragma warning restore 4014
                    return false;
                }
            }
            return true;
        }
        private static List<PokemonList> LegalArray(FriendSafariType type, int index)
        {
            if (type == FriendSafariType.Normal)
            {
                if (index == 0)
                    return new List<PokemonList> { PokemonList.Rattata, PokemonList.Meowth, PokemonList.Lickitung, PokemonList.Sentret, PokemonList.Dunsparce, PokemonList.Teddiursa,
                    PokemonList.Stantler, PokemonList.Zigzagoon, PokemonList.Skitty, PokemonList.Glameow, PokemonList.Patrat, PokemonList.Herdier, PokemonList.Bunnelby};
                if (index == 1)
                    return new List<PokemonList> { PokemonList.Chansey, PokemonList.Eevee, PokemonList.Porygon, PokemonList.Aipom, PokemonList.Smeargle, PokemonList.Miltank,
                    PokemonList.Vigoroth, PokemonList.Loudred, PokemonList.Buneary, PokemonList.Audino, PokemonList.Minccino, PokemonList.Bouffalant, PokemonList.Furfrou};
                if (index == 2)
                {
                    return new List<PokemonList> { PokemonList.Kangaskhan, PokemonList.Tauros, PokemonList.Ditto, PokemonList.Spinda, PokemonList.Zangoose, PokemonList.Castform };
                }
            }
            if (type == FriendSafariType.Fire)
            {
                if (index == 0)
                    return new List<PokemonList> { PokemonList.Ponyta, PokemonList.Magmar, PokemonList.Slugma, PokemonList.Houndour, PokemonList.Numel, PokemonList.Torkoal,
                        PokemonList.Pansear, PokemonList.Heatmor, PokemonList.Litleo};
                if (index == 1)
                    return new List<PokemonList> { PokemonList.Charmeleon, PokemonList.Quilava, PokemonList.Combusken, PokemonList.Monferno, PokemonList.Pignite, PokemonList.Braixen };
                if (index == 2)
                    return new List<PokemonList> { PokemonList.Ninetales, PokemonList.Growlithe, PokemonList.Darumaka, PokemonList.Lampent, PokemonList.Larvesta, PokemonList.Fletchinder };
            }
            if (type == FriendSafariType.Fighting)
            {
                if (index == 0)
                    return new List<PokemonList> { PokemonList.Mankey, PokemonList.Tyrogue, PokemonList.Throh, PokemonList.Sawk, PokemonList.Pancham };
                if (index == 1)
                    return new List<PokemonList> { PokemonList.Poliwhirl, PokemonList.Machoke, PokemonList.Heracross, PokemonList.Combusken, PokemonList.Makuhita, PokemonList.Pignite,
                    PokemonList.Scraggy, PokemonList.Mienfoo, PokemonList.Hawlucha};
                if (index == 2)
                    return new List<PokemonList> { PokemonList.Meditite, PokemonList.Riolu, PokemonList.Croagunk, PokemonList.Gurdurr, PokemonList.Hawlucha };
            }
            if (type == FriendSafariType.Flying)
            {
                if (index == 0)
                    return new List<PokemonList> { PokemonList.Pidgey, PokemonList.Spearow, PokemonList.Farfetchd, PokemonList.Ledyba, PokemonList.Hoppip, PokemonList.Taillow,
                    PokemonList.Wingull, PokemonList.Starly, PokemonList.Combee, PokemonList.Chatot, PokemonList.Pidove, PokemonList.Woobat, PokemonList.Ducklett};
                if (index == 1)
                    return new List<PokemonList> { PokemonList.Golbat, PokemonList.Doduo, PokemonList.Hoothoot, PokemonList.Togetic, PokemonList.Natu, PokemonList.Yanma,
                    PokemonList.Delibird, PokemonList.Swablu, PokemonList.Tropius, PokemonList.Drifloon, PokemonList.Mantyke, PokemonList.Woobat, PokemonList.Emolga};
                if (index == 2)
                    return new List<PokemonList> { PokemonList.Charmeleon, PokemonList.Dragonair, PokemonList.Gligar, PokemonList.Sigilyph, PokemonList.Rufflet, PokemonList.Fletchinder };
            }
            if (type == FriendSafariType.Water)
            {
                if (index == 0)
                    return new List<PokemonList> { PokemonList.Psyduck, PokemonList.Slowpoke, PokemonList.Seel, PokemonList.Krabby, PokemonList.Goldeen, PokemonList.Magikarp,
                    PokemonList.Lapras, PokemonList.Chinchou, PokemonList.Wooper, PokemonList.Qwilfish, PokemonList.Corsola, PokemonList.Remoraid, PokemonList.Lotad, PokemonList.Wingull,
                    PokemonList.Wailmer, PokemonList.Clamperl, PokemonList.Luvdisc, PokemonList.Bidoof, PokemonList.Buizel, PokemonList.Mantyke, PokemonList.Panpour, PokemonList.Tympole,
                    PokemonList.Palpitoad, PokemonList.Basculin, PokemonList.Alomomola, PokemonList.Tirtouga};
                if (index == 1)
                    return new List<PokemonList> { PokemonList.Wartortle, PokemonList.Croconaw, PokemonList.Marshtomp, PokemonList.Prinplup, PokemonList.Dewott, PokemonList.Frogadier };
                if (index == 2)
                    return new List<PokemonList> { PokemonList.Poliwhirl, PokemonList.Tentacool, PokemonList.Seadra, PokemonList.Omanyte, PokemonList.Marill, PokemonList.Corphish,
                    PokemonList.Feebas, PokemonList.Shellos, PokemonList.Ducklett, PokemonList.Frillish, PokemonList.Binacle, PokemonList.Clauncher, PokemonList.Staryu, PokemonList.Carvanha};
            }
            if (type == FriendSafariType.Grass)
            {
                if (index == 0)
                    return new List<PokemonList> { PokemonList.Oddish, PokemonList.Paras, PokemonList.Exeggcute, PokemonList.Hoppip, PokemonList.Sunkern, PokemonList.Lotad, PokemonList.Nuzleaf,
                    PokemonList.Cherubi, PokemonList.Carnivine, PokemonList.Snover, PokemonList.Pansage, PokemonList.Sewaddle, PokemonList.Petilil, PokemonList.Maractus, PokemonList.Foongus, PokemonList.Pumpkaboo};
                if (index == 1)
                    return new List<PokemonList> { PokemonList.Ivysaur, PokemonList.Bayleef, PokemonList.Grovyle, PokemonList.Grotle, PokemonList.Servine, PokemonList.Quilladin };
                if (index == 2)
                    return new List<PokemonList> { PokemonList.Tangela, PokemonList.Shroomish, PokemonList.Roselia, PokemonList.Lileep, PokemonList.Leafeon, PokemonList.Cottonee, PokemonList.Deerling,
                    PokemonList.Ferroseed, PokemonList.Gogoat};
            }
            if (type == FriendSafariType.Poison)
            {
                if (index == 0)
                    return new List<PokemonList> { PokemonList.Weedle, PokemonList.Ekans, PokemonList.Gloom, PokemonList.Weepinbell, PokemonList.Spinarak, PokemonList.Qwilfish, PokemonList.Gulpin, PokemonList.Trubbish };
                if (index == 1)
                    return new List<PokemonList> { PokemonList.Nidorina, PokemonList.Nidorino, PokemonList.Golbat, PokemonList.Venonat, PokemonList.Roselia, PokemonList.Seviper, PokemonList.Stunky, PokemonList.Whirlipede, PokemonList.Foongus };
                if (index == 2)
                    return new List<PokemonList> { PokemonList.Ivysaur, PokemonList.Muk, PokemonList.Koffing, PokemonList.Skorupi, PokemonList.Croagunk, PokemonList.Skrelp };
            }
            if (type == FriendSafariType.Electric)
            {
                if (index == 0)
                    return new List<PokemonList> { PokemonList.Magnemite, PokemonList.Plusle, PokemonList.Minun, PokemonList.Pachirisu, PokemonList.Emolga, PokemonList.Stunfisk, PokemonList.Helioptile, PokemonList.Dedenne };
                if (index == 1)
                    return new List<PokemonList> { PokemonList.Pikachu, PokemonList.Voltorb, PokemonList.Chinchou, PokemonList.Electrike, PokemonList.Luxio, PokemonList.Rotom, PokemonList.Joltik };
                if (index == 2)
                    return new List<PokemonList> { PokemonList.Electabuzz, PokemonList.Jolteon, PokemonList.Flaaffy, PokemonList.Blitzle, PokemonList.Eelektrik };
            }
            if (type == FriendSafariType.Ground)
            {
                if (index == 0)
                    return new List<PokemonList> { PokemonList.Sandshrew, PokemonList.Graveler, PokemonList.Numel, PokemonList.Baltoy, PokemonList.Palpitoad, PokemonList.Stunfisk, PokemonList.Bunnelby };
                if (index == 1)
                    return new List<PokemonList> { PokemonList.Diglett, PokemonList.Cubone, PokemonList.Rhydon, PokemonList.Wooper, PokemonList.Phanpy, PokemonList.Marshtomp, PokemonList.Barboach, PokemonList.Hippopotas, PokemonList.Krokorok,
                    PokemonList.Golett};
                if (index == 2)
                    return new List<PokemonList> { PokemonList.Nidoqueen, PokemonList.Nidoking, PokemonList.Gligar, PokemonList.Piloswine, PokemonList.Vibrava, PokemonList.Gabite, PokemonList.Drilbur };
            }
            if (type == FriendSafariType.Psychic)
            {
                if (index == 0)
                    return new List<PokemonList> { PokemonList.Drowzee, PokemonList.Exeggcute, PokemonList.MrMime, PokemonList.Unown, PokemonList.Spoink, PokemonList.Lunatone, PokemonList.Solrock, PokemonList.Elgyem, PokemonList.Espurr };
                if (index == 1)
                    return new List<PokemonList> { PokemonList.Slowpoke, PokemonList.Jynx, PokemonList.Girafarig, PokemonList.Baltoy, PokemonList.Chimecho, PokemonList.Wynaut, PokemonList.Bronzor, PokemonList.Munna, PokemonList.Woobat, PokemonList.Gothorita };
                if (index == 2)
                    return new List<PokemonList> { PokemonList.Abra, PokemonList.Staryu, PokemonList.Natu, PokemonList.Meditite, PokemonList.Metang, PokemonList.Sigilyph, PokemonList.Duosion };
            }
            if (type == FriendSafariType.Rock)
            {
                if (index == 0)
                    return new List<PokemonList> { PokemonList.Graveler, PokemonList.Sudowoodo, PokemonList.Corsola, PokemonList.Nosepass, PokemonList.Lunatone, PokemonList.Solrock, PokemonList.Boldore };
                if (index == 1)
                    return new List<PokemonList> { PokemonList.Onix, PokemonList.Omanyte, PokemonList.Kabuto, PokemonList.Shuckle, PokemonList.Lileep, PokemonList.Anorith, PokemonList.Cranidos, PokemonList.Shieldon, PokemonList.Binacle, PokemonList.Amaura,
                    PokemonList.Tyrunt};
                if (index == 2)
                    return new List<PokemonList> { PokemonList.Rhydon, PokemonList.Aerodactyl, PokemonList.Pupitar, PokemonList.Lairon, PokemonList.Archen };
            }
            if (type == FriendSafariType.Ice)
            {
                if (index == 0)
                    return new List<PokemonList> { PokemonList.Seel, PokemonList.Snover, PokemonList.Vanillish, PokemonList.Cubchoo, PokemonList.Amaura };
                if (index == 1)
                    return new List<PokemonList> { PokemonList.Jynx, PokemonList.Lapras, PokemonList.Delibird, PokemonList.Snorunt, PokemonList.Cryogonal, PokemonList.Bergmite };
                if (index == 2)
                    return new List<PokemonList> { PokemonList.Shellder, PokemonList.Sneasel, PokemonList.Piloswine, PokemonList.Sealeo };
            }
            if (type == FriendSafariType.Bug)
            {
                if (index == 0)
                    return new List<PokemonList> { PokemonList.Caterpie, PokemonList.Weedle, PokemonList.Paras, PokemonList.Venonat, PokemonList.Ledyba, PokemonList.Spinarak, PokemonList.Wurmple, PokemonList.Surskit, PokemonList.Volbeat,
                    PokemonList.Illumise, PokemonList.Burmy, PokemonList.Scatterbug};
                if (index == 1)
                    return new List<PokemonList> { PokemonList.Yanma, PokemonList.Pineco, PokemonList.Shuckle, PokemonList.Combee, PokemonList.Swadloon, PokemonList.Dwebble, PokemonList.Karrablast, PokemonList.Shelmet, PokemonList.Durant };
                if (index == 2)
                    return new List<PokemonList> { PokemonList.Scyther, PokemonList.Pinsir, PokemonList.Scizor, PokemonList.Heracross, PokemonList.Nincada, PokemonList.Whirlipede, PokemonList.Larvesta };
            }
            if (type == FriendSafariType.Dragon)
            {
                if (index == 0)
                    return new List<PokemonList> { PokemonList.Seadra, PokemonList.Swablu, PokemonList.Skrelp, PokemonList.Tyrunt };
                if (index == 1)
                    return new List<PokemonList> { PokemonList.Vibrava, PokemonList.Fraxure, PokemonList.Druddigon, PokemonList.Sliggoo, PokemonList.Noibat };
                if (index == 2)
                    return new List<PokemonList> { PokemonList.Dragonair, PokemonList.Shelgon, PokemonList.Gabite, PokemonList.Zweilous };
            }
            if (type == FriendSafariType.Ghost)
            {
                if (index == 0)
                    return new List<PokemonList> { PokemonList.Misdreavus, PokemonList.Shedinja, PokemonList.Drifloon, PokemonList.Pumpkaboo  };
                if (index == 1)
                    return new List<PokemonList> { PokemonList.Sableye, PokemonList.Shuppet, PokemonList.Spiritomb, PokemonList.Rotom, PokemonList.Yamask, PokemonList.Golett, PokemonList.Phantump };
                if (index == 2)
                    return new List<PokemonList> { PokemonList.Haunter, PokemonList.Dusclops, PokemonList.Frillish, PokemonList.Lampent, PokemonList.Doublade };
            }
            if (type == FriendSafariType.Dark)
            {
                if (index == 0)
                    return new List<PokemonList> { PokemonList.Poochyena, PokemonList.Nuzleaf, PokemonList.Stunky, PokemonList.Purrloin, PokemonList.Pancham, PokemonList.Inkay };
                if (index == 1)
                    return new List<PokemonList> { PokemonList.Murkrow, PokemonList.Houndour, PokemonList.Cacnea, PokemonList.Corphish, PokemonList.Spiritomb, PokemonList.Krokorok, PokemonList.Scraggy,
                    PokemonList.Zorua};
                if (index == 2)
                    return new List<PokemonList> { PokemonList.Sneasel, PokemonList.Pupitar, PokemonList.Sableye, PokemonList.Skorupi, PokemonList.Pawniard, PokemonList.Vullaby };
            }
            if (type == FriendSafariType.Steel)
            {
                if (index == 0)
                    return new List<PokemonList> { PokemonList.Onix, PokemonList.Nosepass, PokemonList.Shieldon, PokemonList.Bronzor, PokemonList.Karrablast, PokemonList.Klang };
                if (index == 1)
                    return new List<PokemonList> { PokemonList.Magneton, PokemonList.Pineco, PokemonList.Skarmory, PokemonList.Prinplup, PokemonList.Ferroseed, PokemonList.Pawniard, PokemonList.Durant };
                if (index == 2)
                    return new List<PokemonList> { PokemonList.Scizor, PokemonList.Mawile, PokemonList.Lairon, PokemonList.Metang, PokemonList.Riolu, PokemonList.Drilbur };
            }
            if (type == FriendSafariType.Fairy)
            {
                if (index == 0)
                    return new List<PokemonList> { PokemonList.Clefairy, PokemonList.Jigglypuff, PokemonList.Snubbull, PokemonList.Spritzee, PokemonList.Dedenne, PokemonList.Carbink };
                if (index == 1)
                    return new List<PokemonList> { PokemonList.MrMime, PokemonList.Togetic, PokemonList.Kirlia, PokemonList.Cottonee, PokemonList.Swirlix, PokemonList.Sylveon };
                if (index == 2)
                    return new List<PokemonList> { PokemonList.Marill, PokemonList.Mawile, PokemonList.Swablu, PokemonList.Floette, PokemonList.Klefki };
            }
            return new List<PokemonList>();
        }
    }
    public enum FriendSafariType
    {
        None = -1,
        Normal = 0, Fighting, Flying, Poison, Ground, Rock, Bug, Ghost, Steel, Missing, Fire, Water, Grass, Electric,
        Psychic, Ice, Dragon, Dark, Shadow, Bird, Weak, Fairy
    }
    public enum PokemonList : short
    {
        Bulbasaur = 1,
        Ivysaur,
        Venusaur,
        Charmander,
        Charmeleon,
        Charizard,
        Squirtle,
        Wartortle,
        Blastoise,
        Caterpie,
        Metapod,
        Butterfree,
        Weedle,
        Kakuna,
        Beedrill,
        Pidgey,
        Pidgeotto,
        Pidgeot,
        Rattata,
        Raticate,
        Spearow,
        Fearow,
        Ekans,
        Arbok,
        Pikachu,
        Raichu,
        Sandshrew,
        Sandslash,
        NidoranFemale,
        Nidorina,
        Nidoqueen,
        NidoranMale,
        Nidorino,
        Nidoking,
        Clefairy,
        Clefable,
        Vulpix,
        Ninetales,
        Jigglypuff,
        Wigglytuff,
        Zubat,
        Golbat,
        Oddish,
        Gloom,
        Vileplume,
        Paras,
        Parasect,
        Venonat,
        Venomoth,
        Diglett,
        Dugtrio,
        Meowth,
        Persian,
        Psyduck,
        Golduck,
        Mankey,
        Primeape,
        Growlithe,
        Arcanine,
        Poliwag,
        Poliwhirl,
        Poliwrath,
        Abra,
        Kadabra,
        Alakazam,
        Machop,
        Machoke,
        Machamp,
        Bellsprout,
        Weepinbell,
        Victreebel,
        Tentacool,
        Tentacruel,
        Geodude,
        Graveler,
        Golem,
        Ponyta,
        Rapidash,
        Slowpoke,
        Slowbro,
        Magnemite,
        Magneton,
        Farfetchd,
        Doduo,
        Dodrio,
        Seel,
        Dewgong,
        Grimer,
        Muk,
        Shellder,
        Cloyster,
        Gastly,
        Haunter,
        Gengar,
        Onix,
        Drowzee,
        Hypno,
        Krabby,
        Kingler,
        Voltorb,
        Electrode,
        Exeggcute,
        Exeggutor,
        Cubone,
        Marowak,
        Hitmonlee,
        Hitmonchan,
        Lickitung,
        Koffing,
        Weezing,
        Rhyhorn,
        Rhydon,
        Chansey,
        Tangela,
        Kangaskhan,
        Horsea,
        Seadra,
        Goldeen,
        Seaking,
        Staryu,
        Starmie,
        MrMime,
        Scyther,
        Jynx,
        Electabuzz,
        Magmar,
        Pinsir,
        Tauros,
        Magikarp,
        Gyarados,
        Lapras,
        Ditto,
        Eevee,
        Vaporeon,
        Jolteon,
        Flareon,
        Porygon,
        Omanyte,
        Omastar,
        Kabuto,
        Kabutops,
        Aerodactyl,
        Snorlax,
        Articuno,
        Zapdos,
        Moltres,
        Dratini,
        Dragonair,
        Dragonite,
        Mewtwo,
        Mew,
        Chikorita,
        Bayleef,
        Meganium,
        Cyndaquil,
        Quilava,
        Typhlosion,
        Totodile,
        Croconaw,
        Feraligatr,
        Sentret,
        Furret,
        Hoothoot,
        Noctowl,
        Ledyba,
        Ledian,
        Spinarak,
        Ariados,
        Crobat,
        Chinchou,
        Lanturn,
        Pichu,
        Cleffa,
        Igglybuff,
        Togepi,
        Togetic,
        Natu,
        Xatu,
        Mareep,
        Flaaffy,
        Ampharos,
        Bellossom,
        Marill,
        Azumarill,
        Sudowoodo,
        Politoed,
        Hoppip,
        Skiploom,
        Jumpluff,
        Aipom,
        Sunkern,
        Sunflora,
        Yanma,
        Wooper,
        Quagsire,
        Espeon,
        Umbreon,
        Murkrow,
        Slowking,
        Misdreavus,
        Unown,
        Wobbuffet,
        Girafarig,
        Pineco,
        Forretress,
        Dunsparce,
        Gligar,
        Steelix,
        Snubbull,
        Granbull,
        Qwilfish,
        Scizor,
        Shuckle,
        Heracross,
        Sneasel,
        Teddiursa,
        Ursaring,
        Slugma,
        Magcargo,
        Swinub,
        Piloswine,
        Corsola,
        Remoraid,
        Octillery,
        Delibird,
        Mantine,
        Skarmory,
        Houndour,
        Houndoom,
        Kingdra,
        Phanpy,
        Donphan,
        Porygon2,
        Stantler,
        Smeargle,
        Tyrogue,
        Hitmontop,
        Smoochum,
        Elekid,
        Magby,
        Miltank,
        Blissey,
        Raikou,
        Entei,
        Suicune,
        Larvitar,
        Pupitar,
        Tyranitar,
        Lugia,
        HoOh,
        Celebi,
        Treecko,
        Grovyle,
        Sceptile,
        Torchic,
        Combusken,
        Blaziken,
        Mudkip,
        Marshtomp,
        Swampert,
        Poochyena,
        Mightyena,
        Zigzagoon,
        Linoone,
        Wurmple,
        Silcoon,
        Beautifly,
        Cascoon,
        Dustox,
        Lotad,
        Lombre,
        Ludicolo,
        Seedot,
        Nuzleaf,
        Shiftry,
        Taillow,
        Swellow,
        Wingull,
        Pelipper,
        Ralts,
        Kirlia,
        Gardevoir,
        Surskit,
        Masquerain,
        Shroomish,
        Breloom,
        Slakoth,
        Vigoroth,
        Slaking,
        Nincada,
        Ninjask,
        Shedinja,
        Whismur,
        Loudred,
        Exploud,
        Makuhita,
        Hariyama,
        Azurill,
        Nosepass,
        Skitty,
        Delcatty,
        Sableye,
        Mawile,
        Aron,
        Lairon,
        Aggron,
        Meditite,
        Medicham,
        Electrike,
        Manectric,
        Plusle,
        Minun,
        Volbeat,
        Illumise,
        Roselia,
        Gulpin,
        Swalot,
        Carvanha,
        Sharpedo,
        Wailmer,
        Wailord,
        Numel,
        Camerupt,
        Torkoal,
        Spoink,
        Grumpig,
        Spinda,
        Trapinch,
        Vibrava,
        Flygon,
        Cacnea,
        Cacturne,
        Swablu,
        Altaria,
        Zangoose,
        Seviper,
        Lunatone,
        Solrock,
        Barboach,
        Whiscash,
        Corphish,
        Crawdaunt,
        Baltoy,
        Claydol,
        Lileep,
        Cradily,
        Anorith,
        Armaldo,
        Feebas,
        Milotic,
        Castform,
        Kecleon,
        Shuppet,
        Banette,
        Duskull,
        Dusclops,
        Tropius,
        Chimecho,
        Absol,
        Wynaut,
        Snorunt,
        Glalie,
        Spheal,
        Sealeo,
        Walrein,
        Clamperl,
        Huntail,
        Gorebyss,
        Relicanth,
        Luvdisc,
        Bagon,
        Shelgon,
        Salamence,
        Beldum,
        Metang,
        Metagross,
        Regirock,
        Regice,
        Registeel,
        Latias,
        Latios,
        Kyogre,
        Groudon,
        Rayquaza,
        Jirachi,
        Deoxys,
        Turtwig,
        Grotle,
        Torterra,
        Chimchar,
        Monferno,
        Infernape,
        Piplup,
        Prinplup,
        Empoleon,
        Starly,
        Staravia,
        Staraptor,
        Bidoof,
        Bibarel,
        Kricketot,
        Kricketune,
        Shinx,
        Luxio,
        Luxray,
        Budew,
        Roserade,
        Cranidos,
        Rampardos,
        Shieldon,
        Bastiodon,
        Burmy,
        Wormadam,
        Mothim,
        Combee,
        Vespiquen,
        Pachirisu,
        Buizel,
        Floatzel,
        Cherubi,
        Cherrim,
        Shellos,
        Gastrodon,
        Ambipom,
        Drifloon,
        Drifblim,
        Buneary,
        Lopunny,
        Mismagius,
        Honchkrow,
        Glameow,
        Purugly,
        Chingling,
        Stunky,
        Skuntank,
        Bronzor,
        Bronzong,
        Bonsly,
        MimeJr,
        Happiny,
        Chatot,
        Spiritomb,
        Gible,
        Gabite,
        Garchomp,
        Munchlax,
        Riolu,
        Lucario,
        Hippopotas,
        Hippowdon,
        Skorupi,
        Drapion,
        Croagunk,
        Toxicroak,
        Carnivine,
        Finneon,
        Lumineon,
        Mantyke,
        Snover,
        Abomasnow,
        Weavile,
        Magnezone,
        Lickilicky,
        Rhyperior,
        Tangrowth,
        Electivire,
        Magmortar,
        Togekiss,
        Yanmega,
        Leafeon,
        Glaceon,
        Gliscor,
        Mamoswine,
        PorygonZ,
        Gallade,
        Probopass,
        Dusknoir,
        Froslass,
        Rotom,
        Uxie,
        Mesprit,
        Azelf,
        Dialga,
        Palkia,
        Heatran,
        Regigigas,
        Giratina,
        Cresselia,
        Phione,
        Manaphy,
        Darkrai,
        Shaymin,
        Arceus,
        Victini,
        Snivy,
        Servine,
        Serperior,
        Tepig,
        Pignite,
        Emboar,
        Oshawott,
        Dewott,
        Samurott,
        Patrat,
        Watchog,
        Lillipup,
        Herdier,
        Stoutland,
        Purrloin,
        Liepard,
        Pansage,
        Simisage,
        Pansear,
        Simisear,
        Panpour,
        Simipour,
        Munna,
        Musharna,
        Pidove,
        Tranquill,
        Unfezant,
        Blitzle,
        Zebstrika,
        Roggenrola,
        Boldore,
        Gigalith,
        Woobat,
        Swoobat,
        Drilbur,
        Excadrill,
        Audino,
        Timburr,
        Gurdurr,
        Conkeldurr,
        Tympole,
        Palpitoad,
        Seismitoad,
        Throh,
        Sawk,
        Sewaddle,
        Swadloon,
        Leavanny,
        Venipede,
        Whirlipede,
        Scolipede,
        Cottonee,
        Whimsicott,
        Petilil,
        Lilligant,
        Basculin,
        Sandile,
        Krokorok,
        Krookodile,
        Darumaka,
        Darmanitan,
        Maractus,
        Dwebble,
        Crustle,
        Scraggy,
        Scrafty,
        Sigilyph,
        Yamask,
        Cofagrigus,
        Tirtouga,
        Carracosta,
        Archen,
        Archeops,
        Trubbish,
        Garbodor,
        Zorua,
        Zoroark,
        Minccino,
        Cinccino,
        Gothita,
        Gothorita,
        Gothitelle,
        Solosis,
        Duosion,
        Reuniclus,
        Ducklett,
        Swanna,
        Vanillite,
        Vanillish,
        Vanilluxe,
        Deerling,
        Sawsbuck,
        Emolga,
        Karrablast,
        Escavalier,
        Foongus,
        Amoonguss,
        Frillish,
        Jellicent,
        Alomomola,
        Joltik,
        Galvantula,
        Ferroseed,
        Ferrothorn,
        Klink,
        Klang,
        Klinklang,
        Tynamo,
        Eelektrik,
        Eelektross,
        Elgyem,
        Beheeyem,
        Litwick,
        Lampent,
        Chandelure,
        Axew,
        Fraxure,
        Haxorus,
        Cubchoo,
        Beartic,
        Cryogonal,
        Shelmet,
        Accelgor,
        Stunfisk,
        Mienfoo,
        Mienshao,
        Druddigon,
        Golett,
        Golurk,
        Pawniard,
        Bisharp,
        Bouffalant,
        Rufflet,
        Braviary,
        Vullaby,
        Mandibuzz,
        Heatmor,
        Durant,
        Deino,
        Zweilous,
        Hydreigon,
        Larvesta,
        Volcarona,
        Cobalion,
        Terrakion,
        Virizion,
        Tornadus,
        Thundurus,
        Reshiram,
        Zekrom,
        Landorus,
        Kyurem,
        Keldeo,
        Meloetta,
        Genesect,
        Chespin,
        Quilladin,
        Chesnaught,
        Fennekin,
        Braixen,
        Delphox,
        Froakie,
        Frogadier,
        Greninja,
        Bunnelby,
        Diggersby,
        Fletchling,
        Fletchinder,
        Talonflame,
        Scatterbug,
        Spewpa,
        Vivillon,
        Litleo,
        Pyroar,
        Flabébé,
        Floette,
        Florges,
        Skiddo,
        Gogoat,
        Pancham,
        Pangoro,
        Furfrou,
        Espurr,
        Meowstic,
        Honedge,
        Doublade,
        Aegislash,
        Spritzee,
        Aromatisse,
        Swirlix,
        Slurpuff,
        Inkay,
        Malamar,
        Binacle,
        Barbaracle,
        Skrelp,
        Dragalge,
        Clauncher,
        Clawitzer,
        Helioptile,
        Heliolisk,
        Tyrunt,
        Tyrantrum,
        Amaura,
        Aurorus,
        Sylveon,
        Hawlucha,
        Dedenne,
        Carbink,
        Goomy,
        Sliggoo,
        Goodra,
        Klefki,
        Phantump,
        Trevenant,
        Pumpkaboo,
        Gourgeist,
        Bergmite,
        Avalugg,
        Noibat,
        Noivern,
        Xerneas,
        Yveltal,
        Zygarde,
        Diancie,
        Volcanion,
        Hoopa,
        DeltaBulbasaur = 727,
        DeltaIvysaur,
        DeltaVenusaur,
        DeltaCharmander,
        DeltaCharmeleon,
        DeltaCharizard,
        DeltaSquirtle,
        DeltaWartortle,
        DeltaBlastoise,
        DeltaPawniard,
        DeltaBisharp,
        DeltaRalts,
        DeltaKirlia,
        DeltaGardevoir,
        DeltaGallade,
        DeltaSunkern,
        DeltaSunflora,
        DeltaBergmite,
        DeltaAvalugg,
        DeltaScyther,
        DeltaScizor,
        DeltaScraggy,
        DeltaScrafty,
        DeltaCombee,
        DeltaVespiquen,
        DeltaKoffing,
        DeltaWeezing,
        DeltaPurrloin,
        DeltaLiepard,
        DeltaPhantump,
        DeltaTrevenant,
        DeltaSnorunt,
        DeltaGlalie,
        DeltaFroslass,
        DeltaShinx,
        DeltaLuxio,
        DeltaLuxray,
        DeltaNoibat,
        DeltaNoivern,
        DeltaBudew,
        DeltaRoselia,
        DeltaRoserade,
        DeltaGrimer = 771,
        DeltaMuk,
        DeltaWooper,
        DeltaQuagsire,
        DeltaMunchlax,
        DeltaSnorlax,
        DeltaMisdreavus,
        DeltaMismagius
    }

}
