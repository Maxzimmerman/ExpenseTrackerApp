using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.BudgetViewModels;
using ExpenseTrackerApp.Services.IServices;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly ICategoryRepository _categoryRepository;

        public BudgetRepository(
            ITransactionRepository transactionRepository,
            ICategoryRepository categoryRepository,
            ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _transactionRepository = transactionRepository;
            _categoryRepository = categoryRepository;
            _applicationDbContext = applicationDbContext;
        }

        public AddBudgetViewModel addBudgetData(string userId)
        {
            IEnumerable<SelectListItem> categories = _categoryRepository.GetAllCategoriesAsSelectListItems(userId);
                
            var budgets = _applicationDbContext.budgets
                .Include(b => b.Category)
                .Include(b => b.Category.CategoryColor)
                .Include(b => b.Category.CategoryIcon)
                .Where(b => b.Category.ApplicationUserId == userId)
                .ToList();

            AddBudgetViewModel addBudgetViewModel = new AddBudgetViewModel();
            addBudgetViewModel.Categories = categories;
            addBudgetViewModel.Budgets = budgets;
            return addBudgetViewModel;
        }

        public void createBudget(Budget budget)
        {
            var category = _categoryRepository.findCategory(budget.CategoryId);
            if(category == null)
            {
                throw new Exception("Categorie not found.");
            }

            budget.Category = category;
            _applicationDbContext.budgets.Add(budget);
            _applicationDbContext.SaveChanges();
        }

        public void deleteBudget(int id)
        {
            var budget = _applicationDbContext.budgets.FirstOrDefault(b => b.Id == id);
            if(budget != null)
            {
                _applicationDbContext.budgets.Remove(budget);
                _applicationDbContext.SaveChanges();
            }
            else
            {
                throw new Exception("Budget not found.");
            } 
        }
        
        public void updateBudget(Budget budget)
        {
            if (budget == null)
                throw new NullReferenceException("Budget cannot be null.");

            var existingEntity = _applicationDbContext.budgets.Local
                .FirstOrDefault(b => b.Id == budget.Id);

            if (existingEntity != null)
            {
                // Detach the existing entity to prevent conflict
                _applicationDbContext.Entry(existingEntity).State = EntityState.Detached;
            }

            _applicationDbContext.budgets.Update(budget);
            _applicationDbContext.SaveChanges();
        }


        public Budget findBudget(int id)
        {
            var budget = _applicationDbContext.budgets
                .Include(b => b.Category)
                .Include(b => b.Category.CategoryColor)
                .Include(b => b.Category.CategoryIcon)
                .Include(b => b.Category.CategoryType)
                .Include(b => b.Category.ApplicationUser)
                .FirstOrDefault(b => b.Id == id);

            if (budget != null)
                return budget;
            else
                throw new Exception("Couldn't find any budget");
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
                decimal totalBudget = budgetDetail.BudgetAmount;
                if (totalBudget != 0)
                {
                    double spendPercentage = Math.Round((double)(budgetDetail.SpendAmount / totalBudget) * 100, 2);
                    if (spendPercentage > 100)
                        budgetDetail.SpendPercentage = 100;
                    else
                        budgetDetail.SpendPercentage = spendPercentage;
                }
                else
                {
                    // Handle edge case where total budget is zero
                    budgetDetail.SpendPercentage = 0;
                }

                budgetDetail.SpendLastMonth = _transactionRepository.GetSpendForCertainCategoryLastMonth(userId, budgetDetail.Budget.CategoryId);
                budgetDetail.SpendThisMonth = _transactionRepository.GetAmountForCertainCategoryThisMonth(userId, budget.CategoryId);
                budgetDetail.SpendMonthlyAverage = _transactionRepository.GetMonthlyAverageForCertainCategory(userId, budget.CategoryId);

                // chart data budgets and expenses for whole year seperatet in months
                budgetDetail.ExpensesForYear = _transactionRepository.GetExpensesForAllMonthsForCertainCategory(userId, budgetDetail.Budget.CategoryId);
                for(int monthIndex = 1; monthIndex < 13; monthIndex++)
                {
                    budgetDetail.BudgetsForYear.Add(budgetDetail.Budget.Amount);
                }

                // add to view model list
                budgetViewModel.Budgets.Add(budgetDetail);
            }

            return budgetViewModel;
        }

        public async Task<List<Budget>> GetAllBudgets(string userId)
        {
            var budgets = await _applicationDbContext.budgets
                .Include(b => b.Category)
                .Include(b => b.Category.CategoryType)
                .Include(b => b.Category.CategoryIcon)
                .Include(b => b.Category.CategoryColor)
                .Where(b => b.Category.ApplicationUserId == userId)
                .ToListAsync();
            return budgets;
        }

        public decimal GetSumOfAllBudgets(string userId)
        {
            try
            {
                decimal amount = _applicationDbContext.budgets
                    .Include(b => b.Category)
                    .Where(b => b.Category.ApplicationUserId == userId)
                    .Sum(b => b.Amount);
                return amount;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error in GetSumOfAllBudgets: {ex.Message}");
                return 0m;
            }
        }
    }
}
