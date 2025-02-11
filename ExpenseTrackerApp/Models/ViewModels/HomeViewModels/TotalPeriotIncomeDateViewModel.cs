namespace ExpenseTrackerApp.Models.ViewModels;

public class TotalPeriotIncomeDateViewModel
{
    public decimal TotalIncomeAmount { get; set; }
    public decimal IncomeAmountLastMonth { get; set; }
    public decimal DifferenceBetweenThisAndLastMonth { get; set; }
}