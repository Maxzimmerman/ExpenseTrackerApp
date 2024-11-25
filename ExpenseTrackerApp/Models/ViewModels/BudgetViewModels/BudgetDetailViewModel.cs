namespace ExpenseTrackerApp.Models.ViewModels.BudgetViewModels
{
    public class BudgetDetailViewModel
    {
        public Budget Budget { get; set; }
        public decimal SpendAmount { get; set; }
        public decimal BudgetAmount { get; set; }
        public double SpendPercentage { get; set; }
        public double BudgetPercentage { get; set; }
    }
}