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
        private readonly IMessageRepository _messageRepository;

        public BudgetController(
            IBudgetRepository budgetRepository,
            IUserManageService userManageService,
            IFooterRepository footerRepository,
            IMessageRepository messageRepository) : base(footerRepository)
        {
            _budgetRepository = budgetRepository;
            _userManageService = userManageService;
            _messageRepository = messageRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Budgets()
        {
            var budgets = await _budgetRepository.GetBudgetViewModelAsync(_userManageService.GetCurrentUserId(User));

            // Add Message if monthly budget is reached
            foreach(BudgetDetailViewModel budget in budgets.Budgets)
            {
                // message don't exists and budget is reached create a message
                string message = $"Reached budget for {budget.Budget.Category.Title}";
                string userId = _userManageService.GetCurrentUserId(User);
                if (budget.SpendThisMonth >= budget.Budget.Amount && !_messageRepository.ContainsMessageThisMonth(
                    userId, 
                    message,
                    DateTime.Now.Year,
                    DateTime.Now.Month
                    ))
                    _messageRepository.CreateMessageWithUserId(userId, message, "fail", "fi-sr-cross-small");
            }

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
