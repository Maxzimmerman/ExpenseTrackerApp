using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.CategoryViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerApp.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public AddCategory addCategoryData(string userId)
        {
            IEnumerable<SelectListItem> categoryTypes =
                _context.categoriesTypes.Select(ct => new SelectListItem
                {
                    Text = ct.Name,
                    Value = ct.Id.ToString()
                }).ToList();

            IEnumerable<SelectListItem> categoryIcons =
                _context.categoriesIcons.Select(ci => new SelectListItem
                {
                    Text = ci.Name,
                    Value = ci.Id.ToString()
                }).ToList();

            IEnumerable<SelectListItem> categoryColors =
                _context.categoriesColors.Select(cc => new SelectListItem
                {
                    Text = cc.Name,
                    Value = cc.Id.ToString()
                }).ToList();

            var expenses = _context.categories
                .Include(c => c.CategoryType)
                .Include(c => c.CategoryIcon)
                .Include(c => c.CategoryColor)
                .Where(c => c.CategoryType.Name == "Expense" && c.ApplicationUserId == userId)
                .ToList();
            var incoms = _context.categories
                .Include(c => c.CategoryType)
                .Include(c => c.CategoryIcon)
                .Include(c => c.CategoryColor)
                .Where(c => c.CategoryType.Name == "Income" && c.ApplicationUserId == userId)
                .ToList();

            AddCategory addCategory = new AddCategory();
            addCategory.CategoryTypes = categoryTypes;
            addCategory.CategoryIcons = categoryIcons;
            addCategory.CategoryColors = categoryColors;    
            addCategory.Expenses = expenses;
            addCategory.Incomes = incoms;
            
            return addCategory;
        }

        public void createCategory(Category category, string id)
        {
            var categoryIcon = _context.categoriesIcons.FirstOrDefault(c => c.Id == category.CategoryIconId);
            var categoryType = _context.categoriesTypes.FirstOrDefault(c => c.Id == category.CategoryTypeId);
            var categoryColor = _context.categoriesColors.FirstOrDefault(c => c.Id == category.CategoryColorId);

            if (categoryIcon == null)
            {
                throw new Exception($"CategoryIcon with ID {category.CategoryIconId} not found.");
            }

            if (categoryType == null)
            {
                throw new Exception($"CategoryType with ID {category.CategoryTypeId} not found.");
            }

            if (categoryColor == null)
            {
                throw new Exception($"CategoryColor with ID {category.CategoryColorId} not found.");
            }

            category.CategoryIcon = categoryIcon;
            category.CategoryType = categoryType;
            category.CategoryColor = categoryColor;
            category.ApplicationUserId = id;
            _context.categories.Add(category);
            _context.SaveChanges();
        }

        public void deleteCategory(int id)
        {
            var category = this.findCategory(id);
            _context.categories.Remove(category);
            _context.SaveChanges();
        }

        public void updateCategory(Category category)
        {
            _context.categories.Update(category);
            _context.SaveChanges();
        }

        public Category findCategory(int id)
        {
            var category = _context.categories
                .Include(c => c.CategoryType)
                .Include(c => c.CategoryColor)
                .Include(c => c.CategoryIcon)
                .Include(c => c.ApplicationUser)
                .FirstOrDefault(c => c.Id == id);

            if (category != null)
                return category;
            else
                throw new Exception("Couldn't find any category");
        }

        public List<Category> GetAllCategoriesWithTransactions(string userId)
        {
            var list = _context.categories
                .Where(c => c.ApplicationUserId == userId)
                .ToList();

            if (list != null)
                return list;
            else
                throw new Exception("Could not find any category");
        }

        public List<Category> GetAllExpenseCategoriesWithTransactions(string userId)
        {
            var listExpenses = _context.categories
                .Include(c => c.CategoryType)
                .Where(c => c.ApplicationUserId == userId && c.CategoryType.Name == "Expense")
                .ToList();

            if (listExpenses != null)
                return listExpenses;
            else
                throw new Exception("Could not find any category");
        }

        public List<Category> GetAllIncomeCategoriesWithTransactions(string userId)
        {
            var listIncoms = _context.categories
                .Include(c => c.CategoryType)
                .Where(c => c.ApplicationUserId == userId && c.CategoryType.Name == "Income")
                .ToList();

            if (listIncoms != null)
                return listIncoms;
            else
                throw new Exception("Could not find any category");
        }

        public List<Category> GetAllCategoriesWithTransactions(string userId, string ExpenseOrIncom)
        {
            List<Category> categoriesWithTransactions = new List<Category>();
            var categories = _context.categories
                .Include(c => c.CategoryType)
                .Where(c => c.ApplicationUserId == userId && c.CategoryType.Name == ExpenseOrIncom)
                .ToList();

            foreach(var category in categories)
            {
                if(this.CheckIfAllAmountOfACategoriesTransactionsAreAboveZero(userId, category.Title))
                {
                    categoriesWithTransactions.Add(category);
                }
            }

            if (categoriesWithTransactions != null)
                return categoriesWithTransactions;
            else
                throw new Exception("Could not find any category");
        }

        public bool CheckIfAllAmountOfACategoriesTransactionsAreAboveZero(string userId, string categoryName)
        {
            decimal amount = _context.transactions
                .Where(t => t.ApplicationUserId == userId && t.Category.Title == categoryName)
                .Sum(t => t.Amount);

            if (amount != 0)
                return true;
            else
                return false;
        }

        public decimal GetTotalAmountOfAllCategories(string userId, string ExpenseOrIncomd)
        {
            decimal amount = _context.transactions
                .Where(t => t.ApplicationUserId == userId && t.Category.CategoryType.Name == ExpenseOrIncomd)
                .Sum(t => System.Math.Abs(t.Amount));

            return amount;
        }
    }
}