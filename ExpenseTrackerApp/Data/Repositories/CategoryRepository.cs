using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.CategoryViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ExpenseTrackerApp.Data.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CategoryRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public AddCategory addCategoryData(string userId)
        {
            // Todo
            IEnumerable<SelectListItem> categoryTypes =
                _applicationDbContext.categoriesTypes.Select(ct => new SelectListItem
                {
                    Text = ct.Name,
                    Value = ct.Id.ToString()
                }).ToList();

            // Todo
            IEnumerable<SelectListItem> categoryIcons =
                _applicationDbContext.categoriesIcons.Select(ci => new SelectListItem
                {
                    Text = ci.Name,
                    Value = ci.Id.ToString()
                }).ToList();

            // Todo
            IEnumerable<SelectListItem> categoryColors =
                _applicationDbContext.categoriesColors.Select(cc => new SelectListItem
                {
                    Text = cc.Name,
                    Value = cc.Id.ToString()
                }).ToList();

            var expenses = _applicationDbContext.categories
                .Include(c => c.CategoryType)
                .Include(c => c.CategoryIcon)
                .Include(c => c.CategoryColor)
                .Where(c => c.CategoryType.Name == "Expense" && c.ApplicationUserId == userId)
                .ToList();
            var incoms = _applicationDbContext.categories
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
            //Todo
            var categoryIcon = _applicationDbContext.categoriesIcons.FirstOrDefault(c => c.Id == category.CategoryIconId);
            //Todo
            var categoryType = _applicationDbContext.categoriesTypes.FirstOrDefault(c => c.Id == category.CategoryTypeId);
            //Todo
            var categoryColor = _applicationDbContext.categoriesColors.FirstOrDefault(c => c.Id == category.CategoryColorId);

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
            _applicationDbContext.categories.Add(category);
            _applicationDbContext.SaveChanges();
        }

        public void deleteCategory(int id)
        {
            var category = this.findCategory(id);
            _applicationDbContext.categories.Remove(category);
            _applicationDbContext.SaveChanges();
        }

        public void updateCategory(Category category)
        {
            _applicationDbContext.categories.Update(category);
            _applicationDbContext.SaveChanges();
        }

        public Category findCategory(int id)
        {
            var category = _applicationDbContext.categories
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

        public List<Category> GetAllCategories(string userId)
        {
            var categories = _applicationDbContext.categories
                .Where(c => c.ApplicationUserId == userId)
                .ToList();

            if (categories != null)
                return categories;
            else
                throw new Exception("Could not find any category");
        }

        public IEnumerable<SelectListItem> GetAllCategoriesAsSelectListItems(string userId)
        {
            IEnumerable<SelectListItem> categories =
                _applicationDbContext.categories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                });

            if(categories != null)
                return categories;
            else
                throw new Exception("Could not find any category");
        }

        public List<Category> GetAllCategoriesWithTransactions(string userId)
        {
            var list = _applicationDbContext.categories
                .Where(c => c.ApplicationUserId == userId)
                .ToList();

            if (list != null)
                return list;
            else
                throw new Exception("Could not find any category");
        }

        public List<Category> GetAllExpenseCategoriesWithTransactions(string userId)
        {
            var listExpenses = _applicationDbContext.categories
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
            var listIncoms = _applicationDbContext.categories
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
            var categories = _applicationDbContext.categories
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

        //Todo
        public bool CheckIfAllAmountOfACategoriesTransactionsAreAboveZero(string userId, string categoryName)
        {
            decimal amount = _applicationDbContext.transactions
                .Where(t => t.ApplicationUserId == userId && t.Category.Title == categoryName)
                .Sum(t => t.Amount);

            if (amount != 0)
                return true;
            else
                return false;
        }

        //Todo
        public decimal GetTotalAmountOfAllCategories(string userId, string ExpenseOrIncomd)
        {
            decimal amount = _applicationDbContext.transactions
                .Where(t => t.ApplicationUserId == userId && t.Category.CategoryType.Name == ExpenseOrIncomd)
                .Sum(t => System.Math.Abs(t.Amount));

            return amount;
        }

        public int CountAllCategoriesForUser(string userId)
        {
            int categoriesCount = _applicationDbContext.categories.Count();
            return categoriesCount;
        }
    }
}