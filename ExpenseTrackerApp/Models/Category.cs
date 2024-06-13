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

        [ForeignKey("CategoryType")]
        public int CategoryTypeId { get; set; }
        public CategoryType CategoryType { get; set; }

        [ForeignKey("CategoryIcon")]
        public int CategoryIconId { get; set; }
        public CategoryIcon CategoryIcon { get; set; }

        [ForeignKey("CategoryColor")]
        public int CategoryColorId { get; set; }
        public CategoryColor CategoryColor { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

    }
}
