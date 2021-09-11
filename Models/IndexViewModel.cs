using System.ComponentModel.DataAnnotations;

namespace Localization.Models
{
    public class IndexViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
