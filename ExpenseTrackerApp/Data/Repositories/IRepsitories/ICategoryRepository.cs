using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.CategoryViewModels;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        AddCategory addCategoryData(string userId);
        void createCategory(Category category, string id);
        void deleteCategory(int id);
        void updateCategory(Category category);
        Category findCategory(int id);
        public List<Category> GetAllExpenseCategoriesWithTransactions(string userId);
        public List<Category> GetAllIncomeCategoriesWithTransactions(string userId);
        public List<Category> GetAllCategoriesWithTransactions(string userId, string ExpenseOrIncom);
        public bool CheckIfAllAmountOfACategoriesTransactionsAreAboveZero(string userId, string categoryName);
        public decimal GetTotalAmountOfAllCategories(string userId, string ExpenseOrIncomd);
    }
}
