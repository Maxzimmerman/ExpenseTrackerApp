using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.CategoryViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        AddCategory addCategoryData(string userId);
        void createCategory(Category category, string Userid);
        void deleteCategory(int id);
        void updateCategory(Category category);
        Category findCategory(int id);
        List<Category> GetAllCategories(string userId);
        IEnumerable<SelectListItem> GetAllCategoriesAsSelectListItems(string userId);
        List<Category> GetAllExpenseCategoriesWithTransactions(string userId);
        List<Category> GetAllIncomeCategoriesWithTransactions(string userId);
        List<Category> GetAllCategoriesWithTransactions(string userId, string ExpenseOrIncom);
        bool CheckIfAllAmountOfACategoriesTransactionsAreAboveZero(string userId, string categoryName);
        decimal GetTotalAmountOfAllCategories(string userId, string expenseOrIncom);
        int CountAllCategoriesForUser(string userId);
    }
}
