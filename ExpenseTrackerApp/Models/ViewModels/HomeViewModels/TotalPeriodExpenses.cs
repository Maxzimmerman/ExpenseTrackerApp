namespace ExpenseTrackerApp.Models.ViewModels;

public class TotalPeriodExpenses
{
    public decimal TotalAmountOfExpenses { get; set; }
    public decimal AmountOfExpensesLastMonth { get; set; }
    public decimal DifferenceBetweenThisAndLastMonth { get; set; }
}