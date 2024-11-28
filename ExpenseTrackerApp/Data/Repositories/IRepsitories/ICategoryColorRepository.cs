using ExpenseTrackerApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface ICategoryColorRepository : IRepository<CategoryColor>
    {
        IEnumerable<SelectListItem> GetCategoryColorsAsSelectListItems();
        CategoryColor? GetCategoryColorFerCertainCategory(int categoryColorId);
    }
}
