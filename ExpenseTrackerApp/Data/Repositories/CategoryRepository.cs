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

        public AddCategory addCategoryData()
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
                .Where(c => c.CategoryTypeId == 1)
                .ToList();
            var incoms = _context.categories
                .Include(c => c.CategoryType)
                .Include(c => c.CategoryIcon)
                .Include(c => c.CategoryColor)
                .Where(c => c.CategoryTypeId == 2)
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
            category.CategoryIcon = _context.categoriesIcons.FirstOrDefault(c => c.Id == category.CategoryIconId);
            category.CategoryType = _context.categoriesTypes.FirstOrDefault(c => c.Id == category.CategoryTypeId);
            category.CategoryColor = _context.categoriesColors.FirstOrDefault(c => c.Id == category.CategoryColorId);
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
            return _context.categories
                .Include(c => c.CategoryType)
                .Include(c => c.CategoryColor)
                .Include(c => c.CategoryIcon)
                .Include(c => c.ApplicationUser)
                .FirstOrDefault(c => c.Id == id);
        }
    }
}