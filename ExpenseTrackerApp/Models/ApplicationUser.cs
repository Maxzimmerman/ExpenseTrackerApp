using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrackerApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [DefaultValue(0)]
        public decimal Balance { get; set; }

        [Required]
        [MaxLength(100)]
        public string ApplicationUserName { get; set; }

        [Required]
        public DateTime registeredSince { get; set; }
    }
}
