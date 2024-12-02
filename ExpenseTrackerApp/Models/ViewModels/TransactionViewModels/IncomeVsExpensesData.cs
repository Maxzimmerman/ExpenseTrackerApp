namespace ExpenseTrackerApp.Models.ViewModels.TransactionViewModels
{
    public class IncomeVsExpensesData
    {
        public List<ExpenseTrackerApp.Models.Transaction> transactions;
        public List<decimal> incomsChartData;
        public List<decimal> expensesChartData;
    }
}
