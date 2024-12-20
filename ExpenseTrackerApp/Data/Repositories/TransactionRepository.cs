using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.TransactionViewModels;
using System.Diagnostics.Eventing.Reader;

namespace ExpenseTrackerApp.Data.Repositories
{
    public class TransactionRepository : Repository<Models.Transaction>, ITransactionRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ICategoryRepository _categoryRepository;

        public TransactionRepository(ApplicationDbContext applicationDbContext,
            ICategoryRepository categoryRepository) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _categoryRepository = categoryRepository;
        }

        public decimal GetMonthlyAverageForCertainCategory(string userId, int categoryId)
        {
            var currentYear = DateTime.UtcNow.Year;

            var monthlySums = _applicationDbContext.transactions
                .Include(t => t.Category)
                .Where(t => t.CategoryId == categoryId
                         && t.Category.CategoryType.Name == "Expense"
                         && t.ApplicationUserId == userId
                         && t.Date.Year == currentYear)
                .Sum(t => t.Amount);

            decimal monthlyAverage = monthlySums / 12;

            return Math.Round(monthlyAverage, 2);
        }

        public List<decimal> GetExpensesForAllMonthsForCertainCategory(string userId, int categoryId)
        {
            List<decimal> expensesMonths = new List<decimal>();
            int currentYear = DateTime.UtcNow.Year;
            for (int monthIndex = 1; monthIndex < 13; monthIndex++)
            {
                decimal monthExpens = _applicationDbContext.transactions
                    .Include(t => t.Category)
                    .Where(t => t.CategoryId == categoryId
                             && t.Category.CategoryType.Name == "Expense"
                             && t.ApplicationUserId == userId
                             && t.Date.Year == currentYear
                             && t.Date.Month == monthIndex)
                    .Sum(t => t.Amount);
                expensesMonths.Add(monthExpens);
            }
            return expensesMonths;
        }

        public decimal GetIncomeForCertainCategoryLastMonth(string userId, int categoryId)
        {
            int year = DateTime.UtcNow.Year;
            int lastMonth = DateTime.UtcNow.Month - 1;

            decimal amount = _applicationDbContext.transactions
                .Include(t => t.Category)
                .Where(t => t.Category.CategoryType.Name == "Income" &&
                        t.ApplicationUserId == userId &&
                        t.CategoryId == categoryId &&
                        t.Date.Year == year &&
                        t.Date.Month == lastMonth)
                .ToList()
                .Sum(t => t.Amount);
            return Math.Round(amount, 2);
        }

        public decimal GetSpendForCertainCategoryLastMonth(string userId, int categoryId)
        {
            int year = DateTime.UtcNow.Year;
            int month = DateTime.UtcNow.Month - 1;

            decimal amount = _applicationDbContext.transactions
                .Include(t => t.Category)
                .Where(t => t.Category.CategoryType.Name == "Expense" &&
                        t.ApplicationUserId == userId &&
                        t.CategoryId == categoryId &&
                        t.Date.Year == year &&
                        t.Date.Month == month)
                .ToList()
                .Sum(t => t.Amount);
            return Math.Round(amount, 2);
        }

        public decimal GetSpendAmountForCertainCategoryThisMonth(string userId, int categoryId)
        {
            int year = DateTime.UtcNow.Year;
            int month = DateTime.UtcNow.Month;

            decimal amount = _applicationDbContext.transactions
                .Include(t => t.Category)
                .Where(t => t.Category.CategoryType.Name == "Expense" &&
                        t.ApplicationUserId == userId &&
                        t.Category.Id == categoryId &&
                        t.Date.Year == year &&
                        t.Date.Month == month)
                .ToList()
                .Sum(t => t.Amount);
            return Math.Round(amount, 2);
        }

        // General Start

        public List<ExpenseTrackerApp.Models.Transaction> GetExpenses(string userId)
        {
            var expenses = _applicationDbContext.transactions
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
            var incoms = _applicationDbContext.transactions
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
            var transactions = _applicationDbContext.transactions
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

        public List<Models.Transaction> GetTransactionOfCertainMonth(string userId, string ExpenseOrIncome, int month, int year)
        {
            var transactions = _applicationDbContext.transactions
                .Include(t => t.Category)
                .Include(t => t.Category.CategoryColor)
                .Include(t => t.Category.CategoryIcon)
                .Include(t => t.Category.CategoryType)
                .Where(t => t.ApplicationUserId == userId &&
                        t.Date.Month == month &&
                        t.Date.Year == year &&
                        t.Category.CategoryType.Name == ExpenseOrIncome)
                .ToList();

            if (transactions != null)
                return transactions;
            else
                throw new Exception("Could not find any Transactions of certain Month");
        }

        public decimal GetBalanceForCertainMonth(string userId, int month, int year)
        {
            decimal expenses = _applicationDbContext.transactions
                .Where(t => t.ApplicationUserId == userId
                && t.Date.Year == year
                && t.Date.Month == month
                && t.Category.CategoryType.Name == "Expense")
                .Sum(t => t.Amount) * (-1);

            decimal incoms = _applicationDbContext.transactions
                .Where(t => t.ApplicationUserId == userId
                && t.Date.Year == year
                && t.Date.Month == month
                && t.Category.CategoryType.Name == "Income")
                .Sum(t => t.Amount);

            return expenses + incoms;
        }

        public decimal GetBalanceForCertainDay(string userId, int year, int month, int day)
        {
            decimal balance = _applicationDbContext.transactions
                .Where(t => t.ApplicationUserId == userId
                && t.Date.Year == year
                && t.Date.Month == month
                && t.Date.Day == day)
                .Sum(t => t.Amount);

            return balance;
        }

        public decimal GetAmountForTransactionOfCertainWeek(string userid, int year, int month, int week)
        {
            var firstDayOfMonth = new DateTime(year, month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            decimal amount = _applicationDbContext.transactions
                .Where(t => t.ApplicationUserId == userid
                && t.Category.CategoryType.Name == "Expense"
                && t.Date.Year == year
                && t.Date.Month == month
                && t.Date.Day >= firstDayOfMonth.Day && t.Date.Day <= lastDayOfMonth.Day)
                .Sum(t => t.Amount);

            return amount;
        }

        public decimal GetDailyChangeAverageForCurrentMonth(string userId)
        {
            var expenses = _applicationDbContext.transactions
                .Where(t => t.ApplicationUserId == userId
                && t.Category.CategoryType.Name == "Expense"
                && t.Date.Year == DateTime.UtcNow.Year
                && t.Date.Month == DateTime.UtcNow.Month)
                .Sum(t => t.Amount);

            var incoms = _applicationDbContext.transactions
                .Where(t => t.ApplicationUserId == userId
                && t.Category.CategoryType.Name == "Income"
                && t.Date.Year == DateTime.UtcNow.Year
                && t.Date.Month == DateTime.UtcNow.Month)
                .Sum(t => t.Amount);

            // Get the number of days in the current month
            int daysInMonth = DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month);

            if (daysInMonth == 0)
                return 0;

            decimal dailyAverage = (expenses + incoms) / daysInMonth;

            return Math.Round(dailyAverage, 0);
        }


        public decimal GetTotalChangeAmount(string userId)
        {
            decimal expenses = _applicationDbContext.transactions
                .Where(t => t.ApplicationUserId == userId &&
                t.Category.CategoryType.Name == "Expense")
                .Sum(t => t.Amount) * (-1);

            decimal incomes = _applicationDbContext.transactions
                .Where(t => t.ApplicationUserId == userId
                && t.Category.CategoryType.Name == "Income")
                .Sum(t => t.Amount);

            return expenses + incomes;
        }

        public decimal GetTotalSpendAmount(string userId)
        {
            decimal expenses = _applicationDbContext.transactions
                .Include(t => t.Category)
                .Include(t => t.Category.CategoryType)
                .Where(t => t.ApplicationUserId == userId &&
                t.Category.CategoryType.Name == "Expense")
                .Sum(t => t.Amount);
            return expenses;
        }

        // General End
        // Analytics Page Start
        public AnalyticsData GetAnalyticsData(string userId)
        {
            int transactions = _applicationDbContext.transactions.Where(t => t.ApplicationUserId == userId).Count();
            int categories = _categoryRepository.CountAllCategoriesForUser(userId);

            decimal dailyAverage = GetDailyChangeAverageForCurrentMonth(userId);
            decimal totalAmount = GetTotalChangeAmount(userId);
            List<decimal> MonhtlyChanges = new List<decimal>();
            var year = DateTime.UtcNow.Year;

            for (int month = 1; month < 13; month++)
            {
                MonhtlyChanges.Add(GetBalanceForCertainMonth(userId, month, year));
            }

            AnalyticsData data = new AnalyticsData
            {
                TotalTransactions = transactions,
                NumberOfCategories = categories,
                DailyAverageValue = dailyAverage,
                TotalAmount = totalAmount,
                MonhtlyChange = MonhtlyChanges
            };

            return data;
        }

        // Analytics Page End
        // Expense And Income Pages Start
        public decimal GetAmountOfTransactionOfCertainCategory(string CategoryTitle, string ExpenseOrIncome)
        {
            decimal amount = 0;

            amount = _applicationDbContext.transactions
            .Where(t => t.Category.CategoryType.Name == ExpenseOrIncome && t.Category.Title == CategoryTitle)
            .Sum(t => t.Amount);

            if (amount != null)
                return amount;
            else throw new Exception("Could not calc the amount");
        }

        public decimal GetPercentageOfTransactionOfCertainCategory(string CategoryTitle, string userId, string ExpnseOrIncome)
        {
            decimal percentage = 0;
            decimal totalAmountOfCategories = 0;
            decimal amountOfCertainCategory = 0;

            totalAmountOfCategories = _categoryRepository.GetTotalAmountOfAllCategories(userId, ExpnseOrIncome);
            amountOfCertainCategory = this.GetAmountOfTransactionOfCertainCategory(CategoryTitle, ExpnseOrIncome);

            if (totalAmountOfCategories > 0)
                percentage = (amountOfCertainCategory / totalAmountOfCategories) * 100;

            if (percentage > 100)
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

            foreach (var category in _categoryRepository.GetAllExpenseCategoriesWithTransactions(userId))
            {
                ExpenseAndIncomeCategoryData expenseAndIncomeCategoryData = new ExpenseAndIncomeCategoryData();
                expenseAndIncomeCategoryData.Title = category.Title;
                expenseAndIncomeCategoryData.Amount = this.GetAmountOfTransactionOfCertainCategory(category.Title, category.CategoryType.Name);
                expenseAndIncomeCategoryData.Percentage = this.GetPercentageOfTransactionOfCertainCategory(category.Title, userId, category.CategoryType.Name);
                expenseAndIncomeCategoryList.Add(expenseAndIncomeCategoryData);
            }

            expenseAndIncomeData.transactions = expense;
            expenseAndIncomeData.categorieDataList = expenseAndIncomeCategoryList;

            return expenseAndIncomeData;
        }

        public ExpenseAndIncomeData GetIncomeData(string userId)
        {
            ExpenseAndIncomeData expenseAndIncomeData = new ExpenseAndIncomeData();
            List<ExpenseTrackerApp.Models.Transaction> incoms = this.GetIncoms(userId);
            List<ExpenseAndIncomeCategoryData> expenseAndIncomeCategoryList = new List<ExpenseAndIncomeCategoryData>();

            foreach (var category in _categoryRepository.GetAllIncomeCategoriesWithTransactions(userId))
            {
                ExpenseAndIncomeCategoryData expenseAndIncomeCategoryData = new ExpenseAndIncomeCategoryData();
                expenseAndIncomeCategoryData.Title = category.Title;
                expenseAndIncomeCategoryData.Amount = this.GetAmountOfTransactionOfCertainCategory(category.Title, category.CategoryType.Name);
                expenseAndIncomeCategoryData.Percentage = this.GetPercentageOfTransactionOfCertainCategory(category.Title, userId, category.CategoryType.Name);
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
            int year = DateTime.UtcNow.Year;
            var transactions = this.GetTransactions(userId);

            List<decimal> incomeData = new List<decimal>();
            List<decimal> expenseData = new List<decimal>();

            for (int i = 1; i < 13; i++)
            {
                incomeData.Add(GetTransactionOfCertainMonth(userId, "Income", i, year).Sum(t => t.Amount));
                expenseData.Add(GetTransactionOfCertainMonth(userId, "Expense", i, year).Sum(t => t.Amount));
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
        // Balance Page Start

        public BalanceData GetBalanceData(string userId)
        {
            int year = DateTime.UtcNow.Year;
            BalanceData balanceData = new BalanceData();

            List<decimal> monthlyBalance = new List<decimal>();
            List<decimal> daylyBalance = new List<decimal>();
            List<int> daysInCurrentMonth = new List<int>();

            for (int i = 1; i < 13; i++)
            {
                monthlyBalance.Add(GetBalanceForCertainMonth(userId, i, year));
            }

            int currentYear = DateTime.UtcNow.Year;
            int currentMonth = DateTime.UtcNow.Month;
            int numberOfDays = DateTime.DaysInMonth(currentYear, currentMonth);

            for (int i = 1; i < numberOfDays + 1; i++)
            {
                daylyBalance.Add(GetBalanceForCertainDay(userId, currentYear, currentMonth, i));
                daysInCurrentMonth.Add(i);
            }

            balanceData.monthlyBalance = monthlyBalance;
            balanceData.daylyBalance = daylyBalance;
            balanceData.daysInCurrentMonth = daysInCurrentMonth;

            return balanceData;
        }

        // Balance Page End

        public decimal GetTotalAmountForCertainCategory(string userId, string categoryName)
        {
            decimal amount = _applicationDbContext.transactions
                .Where(t => t.ApplicationUserId == userId && t.Category.Title == categoryName)
                .Sum(t => t.Amount);
            return amount;
        }

        public decimal GetTotalAmountForAllCategories(string userId, string expenseOrIncom)
        {
            decimal amount = _applicationDbContext.transactions
                .Where(t => t.ApplicationUserId == userId && t.Category.CategoryType.Name == expenseOrIncom)
                .Sum(t => Math.Abs(t.Amount));

            return amount;
        }
    }
}