namespace ExpenseTrackerApp.Models.ViewModels;

public class TotalBalanceDataViewModel
{
    public decimal TotalBalance { get; set; }
    public decimal DifferenceFromLastToCurrentMonthPercentage { get; set; }
    public decimal BalanceLastMonth { get; set; }
}