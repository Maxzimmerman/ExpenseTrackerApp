using ExpenseTrackerApp.Models.ViewModels.TransactionViewModels;

namespace ExpenseTrackerApp.Models.ViewModels;

public class HomeViewModel
{
    public ApplicationUser User { get; set; }
    public IncomeVsExpensesData IncomeVsExpensesData { get; set; }
    public BalanceTrendsViewModel BalanceTrendsViewModel { get; set; }
}