namespace ExpenseTrackerApp.Models.ViewModels.WalletViewModels;

public class WalletViewModel
{
    public Wallet Wallet { get; set; }
    public TotalBalanceDataViewModel TotalBalanceData { get; set; }
    public TotalPeriodExpensesDataViewModel TotalPeriodExpensesData { get; set; }
    public List<decimal> monthlyBalance;
}