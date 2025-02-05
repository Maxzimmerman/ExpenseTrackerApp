using System.Transactions;
using ExpenseTrackerApp.Models.ViewModels;
using ExpenseTrackerApp.Models.ViewModels.TransactionViewModels;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface ITransactionRepository : IRepository<Models.Transaction>
    {
        // Todo
        BalanceTrendsViewModel getBalanceTrendsData(string userId);

        decimal getMonthlyBalanceAverageForCertainMonthThisYear(string userId, int month);
        // Todo
        List<decimal> GetExpensesForAllMonthsForCertainCategory(string userId, int categoryId);
        decimal GetIncomeForCertainCategoryLastMonth(string userId, int categoryId);
        decimal GetSpendForCertainCategoryLastMonth(string userId, int categoryId);
        decimal GetSpendAmountForCertainCategoryThisMonth(string userId, int categoryId);
        decimal GetMonthlyAverageForCertainCategory(string userId, int categoryId);
        AnalyticsData GetAnalyticsData(string userId);
        List<Models.Transaction> GetExpenses(string userId);
        List<Models.Transaction> GetIncoms(string userId);
        List<Models.Transaction> GetTransactions(string userId);
        ExpenseAndIncomeData GetExpenseData(string userId);
        ExpenseAndIncomeData GetIncomeData(string userId);
        List<Models.Transaction> GetTransactionOfCertainMonth(string userId, string ExpenseOrIncome, int month, int year);
        IncomeVsExpensesData GetIncomeVsExpensesData(string userId);
        decimal GetBalanceForCertainMonth(string userId, int month, int year);
        BalanceData GetBalanceData(string userId);
        decimal GetBalanceForCertainDay(string userId, int year, int month, int week);
        decimal GetAmountForTransactionOfCertainWeek(string userid, int year, int month, int day);
        decimal GetDailyChangeAverageForCurrentMonth(string userId);
        decimal GetTotalChangeAmount(string userId);
        decimal GetTotalSpendAmount(string userId);
        decimal GetTotalAmountForCertainCategory(string userId, string categoryName);
        decimal GetTotalAmountForAllCategories(string userId, string expenseOrIncom);
    }
}
