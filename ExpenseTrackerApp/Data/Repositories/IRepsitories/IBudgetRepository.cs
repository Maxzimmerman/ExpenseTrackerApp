using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.BudgetViewModels;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface IBudgetRepository : IRepository<Budget>
    {
        AddBudgetViewModel addBudgetData(string userId);
        void createBudget(Budget budget);
        void deleteBudget(int id);
        Budget findBudget(int id);
        void updateBudget(Budget budget);
        Task<BudgetViewModel> GetBudgetViewModelAsync(string userId);
        Task<List<Budget>> GetAllBudgets(string userId);
        decimal GetSumOfAllBudgets(string userId);
    }
}
