using System.Transactions;
using ExpenseTrackerApp.Models.ViewModels.TransactionViewModels;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface ITransactionRepository
    {
        ExpenseTrackerApp.Models.Transaction getFirst();
        AnalyticsData GetAnalyticsData(string userId);
        List<ExpenseTrackerApp.Models.Transaction> GetExpenses(string userId);
        List<ExpenseTrackerApp.Models.Transaction> GetIncoms(string userId);
        List<ExpenseTrackerApp.Models.Transaction> GetTransactions(string userId);
        public ExpenseAndIncomeData GetExpenseData(string userId);
        public ExpenseAndIncomeData GetIncomeData(string userId);
        public List<Models.Transaction> GetTransactionOfCertainMonth(string userId, string ExpenseOrIncome, int month);
        public IncomeVsExpensesData GetIncomeVsExpensesData(string userId);
        public decimal GetBalanceForCertainMonth(string userId, int month);
        public BalanceData GetBalanceData(string userId);
        public decimal GetBalanceForCertainDay(string userId, int year, int month, int day);
    }
}
