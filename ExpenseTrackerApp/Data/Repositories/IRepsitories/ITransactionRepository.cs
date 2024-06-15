using System.Transactions;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface ITransactionRepository
    {
        ExpenseTrackerApp.Models.Transaction getFirst();
    }
}
