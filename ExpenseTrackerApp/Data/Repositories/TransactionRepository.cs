using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.TransactionViewModels;
using System.Diagnostics.Eventing.Reader;

namespace ExpenseTrackerApp.Data.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICategoryRepository _categoryRepository;

        public TransactionRepository(ApplicationDbContext context, ICategoryRepository categoryRepository)
        {
            _context = context;
            _categoryRepository = categoryRepository;
        }

        // General Start
        public ExpenseTrackerApp.Models.Transaction getFirst()
        {
            var transaction = _context.transactions
                .Include(t => t.Category)
                .Include(t => t.Category.CategoryType)
                .Include(t => t.Category.CategoryColor)
                .Include(t => t.Category.CategoryIcon)
                .FirstOrDefault(t => t.Id == 3);

            if (transaction != null)
                return transaction;
            else
                throw new Exception("Couldn't find any transaction");
        }

        public List<ExpenseTrackerApp.Models.Transaction> GetExpenses(string userId)
        {
            var expenses = _context.transactions
                .Include(e => e.Category)
                .Include(e => e.Category.CategoryColor)
                .Include(e => e.Category.CategoryIcon)
                .Include(e => e.Category.CategoryType)
                .Where(e => e.ApplicationUserId == userId && e.Category.CategoryType.Name == "Expense")
                .ToList();

            if (expenses != null)
                return expenses;
            else
                throw new Exception("Could not find any expenses");
        }

        public List<ExpenseTrackerApp.Models.Transaction> GetIncoms(string userId)
        {
            var incoms = _context.transactions
                .Include(i => i.Category)
                .Include(i => i.Category.CategoryColor)
                .Include(i => i.Category.CategoryIcon)
                .Include(i => i.Category.CategoryType)
                .Where(i => i.ApplicationUserId == userId && i.Category.CategoryType.Name == "Income")
                .ToList();

            if (incoms != null)
                return incoms;
            else
                throw new Exception("Could not find any expenses");
        }

        public List<ExpenseTrackerApp.Models.Transaction> GetTransactions(string userId)
        {
            var transactions = _context.transactions
                .Include(t => t.Category)
                .Include(t => t.Category.CategoryColor)
                .Include(t => t.Category.CategoryIcon)
                .Include(t => t.Category.CategoryType)
                .Where(t => t.ApplicationUserId == userId)
                .ToList();

            if (transactions != null)
                return transactions;
            else throw new Exception("Could not find any transaction");
        }

        public List<Models.Transaction> GetTransactionOfCertainMonth(string userId, string ExpenseOrIncome, int month)
        {
            var transactions = _context.transactions
                .Include(t => t.Category)
                .Include(t => t.Category.CategoryColor)
                .Include(t => t.Category.CategoryIcon)
                .Include(t => t.Category.CategoryType)
                .Where(t => t.ApplicationUserId == userId && t.Date.Month == month && t.Category.CategoryType.Name == ExpenseOrIncome)
                .ToList();

            if (transactions != null) 
                return transactions; 
            else 
                throw new Exception("Could not find any Transactions of certain Month");
        }

        // General End
        // Analytics Page Start
        public AnalyticsData GetAnalyticsData(string userId)
        {
            int transactions = _context.transactions.ToList().Count;
            int categories = _context.categories.ToList().Count;
            AnalyticsData data = new AnalyticsData(transactions, categories);
            return data;
        }

        // Analytics Page End
        // Expense And Income Pages Start
        public decimal GetAmountOfTransactionOfCertainCategory(string CategoryTitle, string ExpenseOrIncome)
        {
            decimal amount = 0;

            amount = _context.transactions
            .Where(t => t.Category.CategoryType.Name == ExpenseOrIncome && t.Category.Title == CategoryTitle)
            .Sum(t => t.Amount);

            if (amount != null)
                return amount;
            else throw new Exception("Could not calc the amount");
        }

        public decimal GetPercentageOfTransactionOfCertainCetegory(string CategoryTitle, string userId, string ExpnseOrIncome)
        {
            decimal percentage = 0;
            decimal totalAmountOfCategories = 0;
            decimal amountOfCertainCategory = 0;

            totalAmountOfCategories = _categoryRepository.GetTotalAmountOfAllCategories(userId, ExpnseOrIncome);
            amountOfCertainCategory = this.GetAmountOfTransactionOfCertainCategory(CategoryTitle, ExpnseOrIncome);

            if (ExpnseOrIncome == "Expense")
                amountOfCertainCategory *= -1;

            if (totalAmountOfCategories > 0)
                percentage = (amountOfCertainCategory / totalAmountOfCategories) * 100;

            if(percentage > 100)
            {
                percentage = 100;
            }

            return Math.Round(percentage, 0);
        }

        public ExpenseAndIncomeData GetExpenseData(string userId)
        {
            ExpenseAndIncomeData expenseAndIncomeData = new ExpenseAndIncomeData();
            List<ExpenseTrackerApp.Models.Transaction> expense = this.GetExpenses(userId);
            List<ExpenseAndIncomeCategoryData> expenseAndIncomeCategoryList = new List<ExpenseAndIncomeCategoryData>();

            foreach(var category in _categoryRepository.GetAllExpenseCategoriesWithTransactions(userId))
            {
                ExpenseAndIncomeCategoryData expenseAndIncomeCategoryData = new ExpenseAndIncomeCategoryData();
                expenseAndIncomeCategoryData.Title = category.Title;
                expenseAndIncomeCategoryData.Amount = this.GetAmountOfTransactionOfCertainCategory(category.Title, category.CategoryType.Name);
                expenseAndIncomeCategoryData.Percentage = this.GetPercentageOfTransactionOfCertainCetegory(category.Title, userId, category.CategoryType.Name);
                expenseAndIncomeCategoryList.Add(expenseAndIncomeCategoryData);
            }

            expenseAndIncomeData.transactions = expense;
            expenseAndIncomeData.categorieDataList = expenseAndIncomeCategoryList;

            return expenseAndIncomeData;
        }

        public ExpenseAndIncomeData GetIncomeData(string userId)
        {
            ExpenseAndIncomeData expenseAndIncomeData= new ExpenseAndIncomeData();
            List<ExpenseTrackerApp.Models.Transaction> incoms = this.GetIncoms(userId);
            List<ExpenseAndIncomeCategoryData> expenseAndIncomeCategoryList = new List<ExpenseAndIncomeCategoryData>();

            foreach (var category in _categoryRepository.GetAllIncomeCategoriesWithTransactions(userId))
            {
                ExpenseAndIncomeCategoryData expenseAndIncomeCategoryData = new ExpenseAndIncomeCategoryData();
                expenseAndIncomeCategoryData.Title = category.Title;
                expenseAndIncomeCategoryData.Amount = this.GetAmountOfTransactionOfCertainCategory(category.Title, category.CategoryType.Name);
                expenseAndIncomeCategoryData.Percentage = this.GetPercentageOfTransactionOfCertainCetegory(category.Title, userId, category.CategoryType.Name);
                expenseAndIncomeCategoryList.Add(expenseAndIncomeCategoryData);
            }

            expenseAndIncomeData.transactions = incoms;
            expenseAndIncomeData.categorieDataList = expenseAndIncomeCategoryList;

            return expenseAndIncomeData;
        }

        // Expense And Income Pages End
        // Income VS Expenses Page Start

        public IncomeVsExpensesData GetIncomeVsExpensesData(string userId)
        {
            var transactions = this.GetTransactions(userId);

            List<int> incomeData = new List<int>();
            List<int> expenseData = new List<int>();
            
            for(int i = 1; i < 13; i++)
            {
                incomeData.Add(GetTransactionOfCertainMonth(userId, "Income", i).Count);
                expenseData.Add(GetTransactionOfCertainMonth(userId, "Expense", i).Count);
            }

            IncomeVsExpensesData incomeVsExpensesData = new IncomeVsExpensesData();
            incomeVsExpensesData.transactions = transactions;
            incomeVsExpensesData.incomsChartData = incomeData;
            incomeVsExpensesData.expensesChartData = expenseData;

            if (transactions != null)
                return incomeVsExpensesData;
            else
                throw new Exception("Could not collect Income vs Expenses data");
        }

        // Income Vs Expenses Page End
    }
}