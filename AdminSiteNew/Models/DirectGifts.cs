using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminSiteNew.Pokemon;

namespace AdminSiteNew.Models
{
    public class DirectGiftModel
    {
        public string request;
        public List<DirectGiftBase> gifts;
    }

    public class DirectGiftDetailModel
    {
        public string username;
        public int giftIndex;
        public DirectGiftBase gift;
    }

    public class DirectGiftBase
    {
        public DirectGiftType Type;
        public int Index;
        public string Username;

        public void SetType(DirectGiftType type)
        {
            Type = type;
        }
    }

    public enum DirectGiftType
    {
        Unset,
        Pokemon,
        Item
    }

    public class PokemonDirectGift : DirectGiftBase
    {
        public Pokemon Pokemon;
    }

    public class ItemDirectGift : DirectGiftBase
    {
        public ItemList Item;
        public uint Amount;
    }
}
