using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerApp.Models.ViewModels.UserViewModels
{
    public class ResetPasswordViewModel
    {
        [EmailAddress]
        [Required]
        public string EmailAdress { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
    }
}
