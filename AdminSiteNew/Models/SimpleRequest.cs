using System.ComponentModel.DataAnnotations;

namespace AdminSiteNew.Models
{
    public class SimpleRequest
    {
        [Required]
        public string request { get; set; }
    }
}
