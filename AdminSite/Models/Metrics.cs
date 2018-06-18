using System.Collections.Generic;

namespace AdminSite.Models
{
    public class MetricsHolder
    {
        public List<Metrics> List { get; set; }
    }
    public class Metrics
    {
        public int Key { get; set; }
        public string Name { get; set; } = "unset";
        public int Value { get; set; }
    }
}
