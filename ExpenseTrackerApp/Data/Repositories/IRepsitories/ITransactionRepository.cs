using System.Transactions;
using ExpenseTrackerApp.Models.ViewModels.TransactionViewModels;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface ITransactionRepository
    {
        ExpenseTrackerApp.Models.Transaction getFirst();
        AnalyticsData GetAnalyticsData(string userId);
    }
}
