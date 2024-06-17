using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models.ViewModels.TransactionViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTrackerApp.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        [HttpGet]
        public IActionResult Analytics()
        {
            var currentUser = (ClaimsIdentity)User.Identity;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

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
            return View();
        }

        [HttpGet]
        public IActionResult Income()
        {
            return View();
        }

        [HttpGet]
        public IActionResult IncomeVsExpenses()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TransactionHistory()
        {
            return View();
        }
    }
}
