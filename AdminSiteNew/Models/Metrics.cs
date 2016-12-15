using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSiteNew.Models
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
