using System.ComponentModel.DataAnnotations;

namespace InsurgenceServerWebsite.Models
{
    public class SimpleRequest
    {
        [Required]
        public string request { get; set; }
    }
}
