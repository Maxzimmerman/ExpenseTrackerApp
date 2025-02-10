using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models.ViewModels.TransactionViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Data;
using Microsoft.AspNetCore.Identity;
using ExpenseTrackerApp.Services.IServices;

namespace ExpenseTrackerApp.Controllers
{
    [Authorize]
    public class TransactionController : BaseController
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserManageService _userManageService;

        public TransactionController(ITransactionRepository transactionRepository,
            IUserManageService userManageService,
            IFooterRepository footerRepository) : base(footerRepository)
        {
            _transactionRepository = transactionRepository;
            _userManageService = userManageService;
        }

        [HttpGet]
        public IActionResult Analytics()
        {
            AnalyticsData data = _transactionRepository.GetAnalyticsData(_userManageService.GetCurrentUserId(User));
            return View(data);
        }

        [HttpGet]
        public IActionResult Balance()
        {
            var balanceData = _transactionRepository.GetBalanceData(_userManageService.GetCurrentUserId(User));
            return View(balanceData);
        }

        [HttpGet]
        public IActionResult Expenses()
        {
            var expensesData = _transactionRepository.GetExpenseData(_userManageService.GetCurrentUserId(User));
            ViewBag.ChartData = expensesData.categorieDataList;
            return View(expensesData);
        }

        [HttpGet]
        public IActionResult Income()
        {
            var incomsData = _transactionRepository.GetIncomeData(_userManageService.GetCurrentUserId(User));

            return View(incomsData);
        }

        [HttpGet]
        public IActionResult IncomeVsExpenses()
        {
            var incomeVsExpensesData = _transactionRepository.GetIncomeVsExpensesData(_userManageService.GetCurrentUserId(User));

            return View(incomeVsExpensesData);
        }

        [HttpGet]
        public IActionResult TransactionHistory()
        {
            var transactions = _transactionRepository
                .GetTransactions(_userManageService.GetCurrentUserId(User))
                .OrderByDescending(t => t.Date)
                .ToList(); 

            return View(transactions);
        }
    }
}
