using System.Collections.Generic;
using AdminSite.Pokemon;

namespace AdminSite.Models
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
        public virtual DirectGiftType Type { get; }
        public int Index { get; set; }
        public string Username { get; set; }
    }

    public enum DirectGiftType
    {
        Unset,
        Pokemon,
        Item
    }

    public class PokemonDirectGift : DirectGiftBase
    {
        public Pokemon Pokemon { get; set; }
        public override DirectGiftType Type => DirectGiftType.Pokemon;
    }

    public class ItemDirectGift : DirectGiftBase
    {
        public ItemList Item { get; set; }
        public uint Amount { get; set; }
        public override DirectGiftType Type => DirectGiftType.Item;

    }
}
