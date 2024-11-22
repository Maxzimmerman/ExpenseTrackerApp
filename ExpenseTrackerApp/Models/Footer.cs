using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrackerApp.Models
{
    public class Footer
    {
        [Key]
        public int Id { get; set; }
        public string CopryRightHolder { get; set; } = string.Empty;
        [NotMapped]
        public ICollection<SocialLink> SocialLinks { get; set; }
    }
}
