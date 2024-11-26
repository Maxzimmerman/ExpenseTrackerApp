using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.BudgetViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Linq.Expressions;

namespace ExpenseTrackerApp.Data.Repositories
{
    public class BudgetRepository : Repository<Budget>, IBudgetRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ITransactionRepository _transactionRepository;

        public BudgetRepository(
            ITransactionRepository transactionRepository,
            ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _transactionRepository = transactionRepository;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<BudgetViewModel> GetBudgetViewModelAsync(string userId)
        {
            BudgetViewModel budgetViewModel = new BudgetViewModel();
            List<Budget> budgets = await this.GetAllBudgets(userId);

            foreach(Budget budget in budgets)
            {
                BudgetDetailViewModel budgetDetail = new BudgetDetailViewModel();
                // budget
                budgetDetail.Budget = budget;
                
                // amount
                budgetDetail.BudgetAmount = budget.Amount;
                budgetDetail.SpendAmount = _transactionRepository.GetAmountForCertainCategoryThisMonth(userId, budget.CategoryId);

                // percentages
                decimal total = budgetDetail.BudgetAmount;
                if (total != 0)
                {
                    budgetDetail.SpendPercentage = Math.Round((double)(budgetDetail.SpendAmount / total) * 100, 2);
                    budgetDetail.BudgetPercentage = Math.Round(100 - budgetDetail.SpendPercentage, 2);
                }
                else
                {
                    // Handle edge case where total budget is zero
                    budgetDetail.SpendPercentage = 0;
                    budgetDetail.BudgetPercentage = 0;
                }

                budgetDetail.SpendLastMonth = _transactionRepository.GetSpendForCertainCategoryLastMonth(userId, budgetDetail.Budget.CategoryId);

                // add to view model list
                budgetViewModel.Budgets.Add(budgetDetail);
            }

            return budgetViewModel;
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
