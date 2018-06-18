using System;

namespace AdminSite.Models
{
    public class WarningsModel
    {
        public uint Id { get; set; }
        public uint UserId { get; set; }
        public string Username { get; set; }
        public string Reason { get; set; }
        public DateTime Time { get; set; }
    }
}