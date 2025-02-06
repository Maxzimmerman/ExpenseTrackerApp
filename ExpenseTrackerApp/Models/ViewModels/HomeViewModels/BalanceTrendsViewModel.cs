namespace ExpenseTrackerApp.Models.ViewModels;

public class BalanceTrendsViewModel
{
    public List<decimal> Balances { get; set; }
    public decimal Balance { get; set; }
    public decimal BalancePercentage { get; set; }
}