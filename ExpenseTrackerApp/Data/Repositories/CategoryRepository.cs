using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.CategoryViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using ExpenseTrackerApp.Services.IServices;
using Microsoft.Identity.Client;

namespace ExpenseTrackerApp.Data.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ICategoryTypeRepsitory _categoryTypeRepsitory;
        private readonly ICategoryIconRepository _categoryIconRepository;
        private readonly ICategoryColorRepository _categoryColorRepository;
        private readonly Lazy<ITransactionRepository> _transactionRepository;
        
        public CategoryRepository(ApplicationDbContext applicationDbContext,
            ICategoryTypeRepsitory categoryTypeRepsitory,
            ICategoryIconRepository categoryIconRepository,
            ICategoryColorRepository categoryColorRepository,
            Lazy<ITransactionRepository> transactionRepository) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _categoryTypeRepsitory = categoryTypeRepsitory;
            _categoryIconRepository = categoryIconRepository;
            _categoryColorRepository = categoryColorRepository;
            _transactionRepository = transactionRepository;
        }

        public IEnumerable<SelectListItem> GetAllExpenseCategoriesAsSelectListItems(string userId)
        {
            IEnumerable<SelectListItem> categories = _applicationDbContext.categories
                .Where(c => c.ApplicationUserId == userId && c.CategoryType.Name == "Expense")
                .Select(c => new SelectListItem() { Text = c.Title, Value = c.Id.ToString() });
            return categories;
        }

        public AddCategory addCategoryData(string userId)
        {
            IEnumerable<SelectListItem> categoryTypes = _categoryTypeRepsitory.GetCategoryTypesAsSelectListItems();
            IEnumerable<SelectListItem> categoryIcons = _categoryIconRepository.GetCategoryIconsAsSelectListItems();
            IEnumerable<SelectListItem> categoryColors = _categoryColorRepository.GetCategoryColorsAsSelectListItems();

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

        public void createCategory(Category category, string userId)
        {
            var categoryType = _categoryTypeRepsitory.GetCategoryTypeForCertainCategory(category.CategoryTypeId);
            var categoryIcon = _categoryIconRepository.GetCategoryIconById(category.CategoryIconId);
            var categoryColor = _categoryColorRepository.GetCategoryColorFerCertainCategory(category.CategoryColorId);

            if (categoryIcon == null)
            {
                throw new Exception("CategoryIcon not found.");
            }

            if (categoryType == null)
            {
                throw new Exception("CategoryType not found.");
            }

            if (categoryColor == null)
            {
                throw new Exception("CategoryColor not found.");
            }

            category.CategoryIcon = categoryIcon;
            category.CategoryType = categoryType;
            category.CategoryColor = categoryColor;
            category.ApplicationUserId = userId;
            _applicationDbContext.categories.Add(category);
            _applicationDbContext.SaveChanges();
        }

        public void deleteCategory(int id)
        {
            // the findCategory method will throw and exception if id is not valid
            var category = this.findCategory(id);
            _applicationDbContext.categories.Remove(category);
            _applicationDbContext.SaveChanges();
        }

        public void updateCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));
            
            var existingCategory = _applicationDbContext.categories.Local
                .FirstOrDefault(c => c.Id == category.Id);

            if (existingCategory != null)
            {
                _applicationDbContext.Entry(existingCategory).State = EntityState.Detached;
            }
            
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
            throw new Exception("Couldn't find any category");
        }

        public List<Category> GetAllCategories(string userId)
        {
            var categories = _applicationDbContext.categories
                .Where(c => c.ApplicationUserId == userId)
                .ToList();

            return categories;
        }

        public IEnumerable<SelectListItem> GetAllCategoriesAsSelectListItems(string userId)
        {
            IEnumerable<SelectListItem> categories =
                _applicationDbContext.categories.Where(c => c.ApplicationUserId == userId)
                .Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                });

            return categories;
        }

        public List<Category> GetAllExpenseCategories(string userId)
        {
            var listExpenses = _applicationDbContext.categories
                .Include(c => c.CategoryType)
                .Where(c => c.ApplicationUserId == userId && c.CategoryType.Name == "Expense")
                .ToList();

            return listExpenses;
        }

        public List<Category> GetAllIncomeCategories(string userId)
        {
            var listIncoms = _applicationDbContext.categories
                .Include(c => c.CategoryType)
                .Where(c => c.ApplicationUserId == userId && c.CategoryType.Name == "Income")
                .ToList();

            return listIncoms;
        }

        public decimal GetTotalAmountOfAllCategories(string userId, string expenseOrIncom)
        {
            return _transactionRepository.Value.GetTotalAmountForAllCategories(userId, expenseOrIncom);
        }

        public int CountAllCategoriesForUser(string userId)
        {
            int categoriesCount = _applicationDbContext.categories
                .Where(c => c.ApplicationUserId == userId)
                .Count();
            return categoriesCount;
        }

        public int getExpenseDefaultCategoryId(ClaimsPrincipal user)
        {
            try
            {
                return _applicationDbContext.categories
                    .Include(ici => ici.CategoryType)
                    .FirstOrDefault(
                        ici => ici.CategoryType.Name == "Expense" && ici.Title == "Expense Default Category").Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Default Expense Category Not Found it is create {ex.Message}");
                return new Category().Id;
            }
        }

        public int getIncomeDefaultCategoryId(ClaimsPrincipal user)
        {
            try
            {
                return _applicationDbContext.categories
                    .Include(ici => ici.CategoryType)
                    .FirstOrDefault(ici => ici.CategoryType.Name == "Income" && ici.Title == "Income Default Category")
                    .Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Default Income Category Not Found it is create {ex.Message}");
                return new Category().Id;
            }
        }
    }
}