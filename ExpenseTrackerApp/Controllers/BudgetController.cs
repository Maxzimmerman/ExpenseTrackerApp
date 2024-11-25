using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models.ViewModels.BudgetViewModels;
using ExpenseTrackerApp.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerApp.Controllers
{
    public class BudgetController : BaseController
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUserManageService _userManageService;

        public BudgetController(
            IBudgetRepository budgetRepository,
            IUserManageService userManageService,
            IFooterRepository footerRepository) : base(footerRepository)
        {
            _budgetRepository = budgetRepository;
            _userManageService = userManageService;
        }

        [HttpGet]
        public async Task<IActionResult> Budgets()
        {
            string userId = _userManageService.GetCurrentUserId(User);
            var budgets = await _budgetRepository.GetBudgetViewModelAsync(userId);
            return View(budgets);
        }
    }
}
