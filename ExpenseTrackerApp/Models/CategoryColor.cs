using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerApp.Models
{
    public class CategoryColor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string code { get; set; }
    }
}
