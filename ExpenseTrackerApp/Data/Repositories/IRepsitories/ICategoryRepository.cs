using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.RepositoryModels;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface ICategoryRepository
    {
        AddCategory addCategoryData();
        void createCategory(Category category, string id);
        void deleteCategory(int id);
        void updateCategory(Category category);
        Category findCategory(int id);
    }
}
