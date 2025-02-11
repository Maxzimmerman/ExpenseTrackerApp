using System.Transactions;
using ExpenseTrackerApp.Models.ViewModels;
using ExpenseTrackerApp.Models.ViewModels.TransactionViewModels;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface ITransactionRepository : IRepository<Models.Transaction>
    {
        BalanceTrendsViewModel getBalanceTrendsData(string userId);
        decimal getMonthlyBalanceForCertainMonthThisYear(string userId, int month);
        List<MonthyBudgetEntryViewModel> getMonthlyBudgetData(string userId);
        List<decimal> GetExpensesForAllMonthsForCertainCategory(string userId, int categoryId);
        List<ExpenseAndIncomeCategoryData> getMonthlyExpenseBreakDown(string userId);
        decimal GetPercentageOfTransactionOfCertainCategoryThisMonth(string userId, string ExpnseOrIncome, int categoryId);
        decimal GetExpenseTotalAmountForAllCategoriesThisMonth(string userId);
        TotalBalanceDataViewModel getTotalBalanceData(string userId);
        TotalPeriodExpensesDataViewModel getTotalPeriodExpensesData(string userId);
        decimal GetExpenseTotalAmountForAllCategoriesLastMonth(string userId);
        // Todo
        TotalPeriotIncomeDateViewModel getTotalIncomeData(string userId);
        decimal GetIncomeTotalAmountForAllCategoriesThisMonth(string userId);
        decimal GetIncomeTotalAmountForAllCategoriesLastMonth(string userId);

        decimal GetTotalIncomeAmount(string userId);
        // Todo
        decimal GetIncomeForCertainCategoryLastMonth(string userId, int categoryId);
        decimal GetSpendForCertainCategoryLastMonth(string userId, int categoryId);
        decimal GetSpendAmountForCertainCategoryThisMonth(string userId, int categoryId);
        decimal GetSpendMonthlyAverageForCertainCategory(string userId, int categoryId);
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
        decimal GetAmountSpendForTransactionOfCertainWeek(string userid, int year, int month, int day);
        decimal GetDailyBalanceAverageForCurrentMonthThisYear(string userId);
        decimal GetTotalBalanceAmount(string userId);
        decimal GetTotalSpendAmount(string userId);
        decimal GetTotalAmountForCertainCategory(string userId, string categoryName);
        decimal GetTotalAmountForAllCategories(string userId, string expenseOrIncom);
        // Todo
    }
}