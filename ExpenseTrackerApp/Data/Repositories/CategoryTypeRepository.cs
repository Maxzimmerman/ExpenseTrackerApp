using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTrackerApp.Data.Repositories
{
    public class CategoryTypeRepository : Repository<CategoryType>, ICategoryTypeRepsitory
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CategoryTypeRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IEnumerable<SelectListItem> GetCategoryTypesAsSelectListItems()
        {
            IEnumerable<SelectListItem> categoryTypes = _applicationDbContext.categoriesTypes.Select(ct => new SelectListItem
            {
                Text = ct.Name,
                Value = ct.Id.ToString()
            }).ToList();

            return categoryTypes;
        }

        public CategoryType? GetCategoryTypeForCertainCategory(int categoryTypeId)
        {
            var categoryType = _applicationDbContext.categoriesTypes.FirstOrDefault(c => c.Id == categoryTypeId);
            if (categoryType != null)
                return categoryType;
            throw new Exception("Could not find CategoryType");
        }
    }
}
