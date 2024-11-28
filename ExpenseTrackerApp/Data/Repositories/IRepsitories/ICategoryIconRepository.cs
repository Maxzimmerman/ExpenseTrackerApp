using ExpenseTrackerApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface ICategoryIconRepository : IRepository<CategoryIcon>
    {
        IEnumerable<SelectListItem> GetCategoryIconsAsSelectListItems();
        CategoryIcon? GetCategoryBelongingToCertainCategory(int categoryIconId);
    }
}
