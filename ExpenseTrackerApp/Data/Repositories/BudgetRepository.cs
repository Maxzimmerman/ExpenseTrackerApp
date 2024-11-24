using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ExpenseTrackerApp.Data.Repositories
{
    public class BudgetRepository : Repository<Budget>, IBudgetRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public BudgetRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<List<Budget>> GetAllBudgets(string userId)
        {
            var budgets = await _applicationDbContext.budgets
                .Include(b => b.Category)
                .Include(b => b.BudgetType)
                .Include(b => b.Category.CategoryType)
                .Include(b => b.Category.CategoryIcon)
                .Include(b => b.Category.CategoryColor)
                .Where(b => b.Category.ApplicationUserId == userId)
                .ToListAsync();
            return budgets;
        }
    }
}
