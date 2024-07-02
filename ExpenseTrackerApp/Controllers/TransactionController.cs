using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models.ViewModels.TransactionViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Data;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTrackerApp.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public TransactionController(ITransactionRepository transactionRepository, UserManager<IdentityUser> userManager)
        {
            _transactionRepository = transactionRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Analytics()
        {
            var currentUser = await _userManager.FindByIdAsync(User.Claims.First().Value);
            var currentUserId = currentUser.Id;

            AnalyticsData data = _transactionRepository.GetAnalyticsData(currentUserId);

            return View(data);
        }

        [HttpGet]
        public IActionResult Balance()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Expenses()
        {
            var expensesData = _transactionRepository.GetExpenseData(User.Claims.First().Value);

            ViewBag.ChartData = expensesData.categorieDataList;

            return View(expensesData);
        }

        [HttpGet]
        public IActionResult Income()
        {
            var incomsData = _transactionRepository.GetIncomeData(User.Claims.First().Value);

            return View(incomsData);
        }

        [HttpGet]
        public IActionResult IncomeVsExpenses()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TransactionHistory()
        {
            var transactions = _transactionRepository.GetTransactions(User.Claims.First().Value); 

            return View(transactions);
        }
    }
}
