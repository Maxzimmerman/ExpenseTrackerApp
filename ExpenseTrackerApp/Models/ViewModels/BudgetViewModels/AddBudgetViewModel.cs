using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTrackerApp.Models.ViewModels.BudgetViewModels
{
    public class AddBudgetViewModel
    {
        public IEnumerable<SelectListItem> Categories { get; set; }
        public List<Budget> Budgets { get; set; }
    }
}
