using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerApp.Models
{
    public class Footer
    {
        [Key]
        public int Id { get; set; }
        public string CopryRightHolder { get; set; } = string.Empty;
        public List<SocialLinks> Links { get; set; }
    }
}
