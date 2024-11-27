using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.BudgetViewModels;
using ExpenseTrackerApp.Models.ViewModels.CategoryViewModels;
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

        [HttpGet]
        public IActionResult SettingsBudget()
        {
            AddBudgetViewModel budgetViewModel = _budgetRepository.addBudgetData(_userManageService.GetCurrentUserId(User));

            ViewBag.Budgets = budgetViewModel.Budgets;
            ViewBag.Categories = budgetViewModel.Categories;

            return View();
        }

        [HttpPost]
        public IActionResult AddBudget(Budget budget)
        {
            if(budget == null)
            {
                return RedirectToAction("BadRequest");
            }
            else
            {
                _budgetRepository.createBudget(budget);
                return RedirectToAction("Home", "Home");
            }
        }

        [HttpGet]
        public IActionResult LoadUpdateBudget(int id)
        {
            var budget = _budgetRepository.findBudget(id);

            AddBudgetViewModel addBudgetViewModel = _budgetRepository.addBudgetData(_userManageService.GetCurrentUserId(User));

            ViewBag.Categories = addBudgetViewModel.Categories;
            ViewBag.Budgets = addBudgetViewModel.Budgets.ToList();
            ViewBag.LoadedBudget = budget;

            return View("SettingsBudget", budget);
        }

        [HttpPost]
        public IActionResult UpdateBudget(Budget budget)
        {
            _budgetRepository.updateBudget(budget);
            return RedirectToAction("SettingsBudget");
        }

        [HttpGet]
        public IActionResult DeleteBudget(int id)
        {
            _budgetRepository.deleteBudget(id);
            return RedirectToAction("SettingsBudget");
        }
    }
}
