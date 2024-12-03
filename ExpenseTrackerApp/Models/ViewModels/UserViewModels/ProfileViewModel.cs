namespace ExpenseTrackerApp.Models.ViewModels.UserViewModels
{
    public class ProfileViewModel
    {
        public ApplicationUser User { get; set; } = new ApplicationUser();
        public decimal TotalBudget { get; set; } = 0;
        public decimal TotalSpend { get; set; } = 0;
        public double SpendPercentage { get; set; } = 0;
    }
}
