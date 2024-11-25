using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.BudgetViewModels;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface IBudgetRepository : IRepository<Budget>
    {
        Task<BudgetViewModel> GetBudgetViewModelAsync(string userId);
        Task<List<Budget>> GetAllBudgets(string userId);
    }
}
