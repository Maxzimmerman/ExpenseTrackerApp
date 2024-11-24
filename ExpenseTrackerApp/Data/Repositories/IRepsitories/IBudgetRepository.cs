using ExpenseTrackerApp.Models;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface IBudgetRepository : IRepository<Budget>
    {
        Task<List<Budget>> GetAllBudgets(string userId);
    }
}
