using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.CategoryViewModels;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface ICategoryRepository
    {
        AddCategory addCategoryData(string userId);
        void createCategory(Category category, string id);
        void deleteCategory(int id);
        void updateCategory(Category category);
        Category findCategory(int id);
        public List<Category> GetAllCategories(string userId);
        public List<Category> GetAllExpenseCategories(string userId);
        public List<Category> GetAllIncomeCategories(string userId);    
    }
}
