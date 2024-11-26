using System.Transactions;
using ExpenseTrackerApp.Models.ViewModels.TransactionViewModels;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface ITransactionRepository : IRepository<Models.Transaction>
    {
        decimal GetIncomeForCertainCategoryLastMonth(string userId, int categoryId);
        decimal GetSpendForCertainCategoryLastMonth(string userId, int categoryId);
        decimal GetAmountForCertainCategoryThisMonth(string userId, int categoryId);
        AnalyticsData GetAnalyticsData(string userId);
        List<ExpenseTrackerApp.Models.Transaction> GetExpenses(string userId);
        List<ExpenseTrackerApp.Models.Transaction> GetIncoms(string userId);
        List<ExpenseTrackerApp.Models.Transaction> GetTransactions(string userId);
        public ExpenseAndIncomeData GetExpenseData(string userId);
        public ExpenseAndIncomeData GetIncomeData(string userId);
        public List<Models.Transaction> GetTransactionOfCertainMonth(string userId, string ExpenseOrIncome, int month, int year);
        public IncomeVsExpensesData GetIncomeVsExpensesData(string userId);
        public decimal GetBalanceForCertainMonth(string userId, int month, int year);
        public BalanceData GetBalanceData(string userId);
        public decimal GetBalanceForCertainDay(string userId, int year, int month, int week);
        public decimal GetAmountForTransactionOfCertainWeek(string userid, int year, int month, int day);
        public decimal GetDailyAverage(string userId);
        public decimal GetTotalAmount(string userId);
    }
}
