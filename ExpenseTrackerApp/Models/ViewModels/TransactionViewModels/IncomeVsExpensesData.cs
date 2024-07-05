namespace ExpenseTrackerApp.Models.ViewModels.TransactionViewModels
{
    public class IncomeVsExpensesData
    {
        public List<ExpenseTrackerApp.Models.Transaction> transactions;
        public List<Dictionary<string, int>> incomsChartData;
        public List<Dictionary<string, int>> expensesChartData;
    }
}
