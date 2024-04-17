using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrackerApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Title { get; set; }

        [Required]
        public decimal monthlyBudget { get; set; }

        [Required]
        [ForeignKey("CategoryType")]
        public int CateogryTypeId { get; set; }
        public CategoryType CategoryType { get; set; }

        [Required]
        [ForeignKey("CategoryIcon")]
        public int CategoryIconId { get; set; }
        public CategoryIcon CategoryIcon { get; set; }

        [Required]
        [ForeignKey("CategoryColor")]
        public int CategoryColorId { get; set; }
        public CategoryColor CategoryColor { get; set; }
    }
}
