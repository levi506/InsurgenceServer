using System.ComponentModel.DataAnnotations;

namespace AdminSite.Models
{
    public class SimpleRequest
    {
        [Required]
        public string request { get; set; }
    }
}
