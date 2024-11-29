using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerApp.Models.ViewModels.UserViewModels
{
    public class SendResetEmailModelView
    {
        [EmailAddress]
        [Required]
        public string EmailAddress { get; set; } = string.Empty;
    }
}
