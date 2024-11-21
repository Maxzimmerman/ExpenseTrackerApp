using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrackerApp.Models
{
    public class SocialLinks
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Platform { get; set; } = string.Empty;
        [Required]
        public string Url { get; set; } = string.Empty;
        [Required]
        public string IconClass { get; set; } = string.Empty;

        [ForeignKey("Footer")]
        public int FooterId { get; set; }
        public Footer Footer { get; set; }
    }
}
