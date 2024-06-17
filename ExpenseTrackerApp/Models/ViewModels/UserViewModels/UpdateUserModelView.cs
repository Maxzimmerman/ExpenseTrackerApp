using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerApp.Models.ViewModels.UserViewModels
{
    public class UpdateUserModelView
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at leat {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Registered since")]
        public DateTime registerdSince { get; set; }
    }
}
