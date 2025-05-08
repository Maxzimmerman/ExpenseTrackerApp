using System.Security.Claims;
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
        IEnumerable<SelectListItem> GetAllExpenseCategoriesAsSelectListItems(string userId);
        List<Category> GetAllExpenseCategories(string userId);
        List<Category> GetAllIncomeCategories(string userId);
        decimal GetTotalAmountOfAllCategories(string userId, string expenseOrIncom);
        int CountAllCategoriesForUser(string userId);
        int getExpenseDefaultCategoryId(ClaimsPrincipal user);
        int getIncomeDefaultCategoryId(ClaimsPrincipal user);
        Category getExpenseDefaultCategory(string userId);
        Category getIncomeDefaultCategory(string userId);
    }
}