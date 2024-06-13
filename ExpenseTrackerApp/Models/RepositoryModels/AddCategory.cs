using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTrackerApp.Models.RepositoryModels
{
    public class AddCategory
    {
        public IEnumerable<SelectListItem> CategoryTypes { get; set; }
        public IEnumerable<SelectListItem> CategoryIcons { get; set; }
        public IEnumerable<SelectListItem> CategoryColors { get; set; }
        public List<Category> Expenses { get; set; }
        public List<Category> Incomes { get; set; }
    }
}
