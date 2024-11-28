using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTrackerApp.Data.Repositories
{
    public class CategoryIconRepository : Repository<CategoryIcon>, ICategoryIconRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CategoryIconRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IEnumerable<SelectListItem> GetCategoryIconsAsSelectListItems()
        {
            IEnumerable<SelectListItem> categoryIcons = _applicationDbContext.categoriesIcons.Select(ci => new SelectListItem
            {
                Text = ci.Name,
                Value = ci.Id.ToString()
            }).ToList();

            return categoryIcons;
        }

        public CategoryIcon? GetCategoryBelongingToCertainCategory(int categoryIconId)
        {
            var categoryIcon = _applicationDbContext.categoriesIcons.FirstOrDefault(c => c.Id == categoryIconId);
            if (categoryIcon != null)
                return categoryIcon;
            else
                throw new Exception("Could not find CategoryIcon");
        }
    }
}
