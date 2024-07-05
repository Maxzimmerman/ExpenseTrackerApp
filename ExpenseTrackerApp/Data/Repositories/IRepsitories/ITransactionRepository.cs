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
        public List<ExpenseTrackerApp.Models.Transaction> GetTransactionsForJan(string userId);
        public List<ExpenseTrackerApp.Models.Transaction> GetTransactionsForFeb(string userId);
        public List<ExpenseTrackerApp.Models.Transaction> GetTransactionsForMar(string userId);
        public List<ExpenseTrackerApp.Models.Transaction> GetTransactionsForApr(string userId);
        public List<ExpenseTrackerApp.Models.Transaction> GetTransactionsForMay(string userId);
        public List<ExpenseTrackerApp.Models.Transaction> GetTransactionsForJun(string userId);
        public List<ExpenseTrackerApp.Models.Transaction> GetTransactionsForJul(string userId);
        public List<ExpenseTrackerApp.Models.Transaction> GetTransactionsForAug(string userId);
        public List<ExpenseTrackerApp.Models.Transaction> GetTransactionsForSep(string userId);
        public List<ExpenseTrackerApp.Models.Transaction> GetTransactionsForOct(string userId);
        public List<ExpenseTrackerApp.Models.Transaction> GetTransactionsForNov(string userId);
        public List<ExpenseTrackerApp.Models.Transaction> GetTransactionsForDec(string userId);
        public IncomeVsExpensesData GetIncomeVsExpensesData(string userId);
    }
}
