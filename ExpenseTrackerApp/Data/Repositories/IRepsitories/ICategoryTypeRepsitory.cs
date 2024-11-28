using ExpenseTrackerApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface ICategoryTypeRepsitory : IRepository<CategoryType>
    {
        IEnumerable<SelectListItem> GetCategoryTypesAsSelectListItems();
        CategoryType? GetCategoryTypeForCertainCategory(int categoryTypeId);
    }
}
