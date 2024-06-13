using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerApp.Models
{
    public class CategoryIcon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
    }
}
