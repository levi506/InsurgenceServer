using System.Collections.Generic;

namespace AdminSite.Models
{
    public class GTSListModel
    {
        public uint StartIndex { get; set; }
        public List<GTSObject> GTS { get; set; }
    }

    public class GTSObject
    {
        public int Id { get; set; }
        public Pokemon Offer { get; set; }
        public GTSFilter Request { get; set; }
        public uint UserId { get; set; }
        public string OwnerName { get; set; }
        public string TraderName { get; set; }
        public bool Accepted { get; set; }
        public Pokemon Result { get; set; }
    }


    public class GTSFilter
    {
        public int MinLevel { get; set; } = 0;
        public int Species { get; set; } = 0;
        public int Nature { get; set; } = 25;
        public int Gender { get; set; } = 0;
    }
}
