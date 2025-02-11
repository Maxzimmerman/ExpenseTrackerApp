using ExpenseTrackerApp.Models.ViewModels.TransactionViewModels;

namespace ExpenseTrackerApp.Models.ViewModels;

public class HomeViewModel
{
    public ApplicationUser User { get; set; }
    public IncomeVsExpensesData IncomeVsExpensesData { get; set; }
    public BalanceTrendsViewModel BalanceTrendsViewModel { get; set; }
    public List<MonthyBudgetEntryViewModel> MonthyBudgetEntries { get; set; }
    public List<ExpenseAndIncomeCategoryData> ExpenseAndIncomeCategories { get; set; }
    public TotalBalanceDataViewModel TotalBalanceDataViewModel { get; set; }
    public TotalPeriodExpensesDataViewModel TotalPeriodExpensesDataViewModel { get; set; }
    public TotalPeriotIncomeDateViewModel TotalPeriotIncomeDateViewModel { get; set; }
}