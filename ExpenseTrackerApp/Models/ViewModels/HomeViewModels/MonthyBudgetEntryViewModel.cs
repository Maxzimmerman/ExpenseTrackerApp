namespace ExpenseTrackerApp.Models.ViewModels;

public class MonthyBudgetEntryViewModel
{
    public string Name { get; set; }
    public decimal BudgetAmount { get; set; }
    public decimal SpendAmount { get; set; }
    public decimal SpendPercentage { get; set; }
}