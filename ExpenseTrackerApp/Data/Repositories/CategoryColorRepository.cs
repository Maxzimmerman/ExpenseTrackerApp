using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTrackerApp.Data.Repositories
{
    public class CategoryColorRepository : Repository<CategoryColor>, ICategoryColorRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CategoryColorRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IEnumerable<SelectListItem> GetCategoryColorsAsSelectListItems()
        {
            IEnumerable<SelectListItem> categoryColors = _applicationDbContext.categoriesColors.Select(cc => new SelectListItem
            {
                Text = cc.Name,
                Value = cc.Id.ToString()
            });

            return categoryColors;
        }

        public CategoryColor? GetCategoryColorFerCertainCategory(int categoryColorId)
        {
            var categoryColor = _applicationDbContext.categoriesColors.FirstOrDefault(c => c.Id == categoryColorId);
            if (categoryColor != null)
                return categoryColor;
            else
                throw new Exception("Could not find CategoryColor");
        }
    }
}
