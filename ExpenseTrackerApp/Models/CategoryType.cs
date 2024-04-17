using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerApp.Models
{
    public class CategoryType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(7)]
        public string Name { get; set; }
    }
}
