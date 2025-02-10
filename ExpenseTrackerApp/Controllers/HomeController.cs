using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using ExpenseTrackerApp.Models.ViewModels;

namespace ExpenseTrackerApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IUserManageService _userManageService;
        private readonly IMessageRepository _messageRepository;
        private readonly ITransactionRepository _transactionRepository;

        public HomeController(ILogger<HomeController> logger,
            IUserRepository userRepository,
            IUserManageService userManageService,
            IFooterRepository footerRepository,
            IMessageRepository messageRepository,
            ITransactionRepository transactionRepository) : base(footerRepository)
        {
            _logger = logger;
            _userManageService = userManageService;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _transactionRepository = transactionRepository;
        }

        public IActionResult Home()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = _userRepository.getUserById(_userManageService.GetCurrentUserId(User));
                HomeViewModel homeViewModel = new HomeViewModel();
                homeViewModel.User = user;
                homeViewModel.IncomeVsExpensesData = _transactionRepository.GetIncomeVsExpensesData(user.Id);
                homeViewModel.BalanceTrendsViewModel = _transactionRepository.getBalanceTrendsData(user.Id);
                homeViewModel.MonthyBudgetEntries = _transactionRepository.getMonthlyBudgetData(user.Id);
                homeViewModel.ExpenseAndIncomeCategories = _transactionRepository.getMonthlyExpenseBreakDown(user.Id);
                return View(homeViewModel);
            }
            return RedirectToAction("SignIn", "UserManage");
        }

        [Authorize]
        public IActionResult Budgets()
        {
            return View();
        }

        [Authorize]
        public IActionResult Goals()
        {
            return View();
        }

        [Authorize]
        public IActionResult Analytics()
        {
            return View();
        }

        [Authorize]
        public IActionResult Settings()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}