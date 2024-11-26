using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models.ViewModels.BudgetViewModels;
using ExpenseTrackerApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerApp.Controllers
{
    [Authorize]
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
            var budgets = await _budgetRepository.GetBudgetViewModelAsync(_userManageService.GetCurrentUserId(User));
            return View(budgets);
        }
    }
}
