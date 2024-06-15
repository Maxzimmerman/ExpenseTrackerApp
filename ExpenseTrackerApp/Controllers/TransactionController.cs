using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerApp.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public IActionResult Analytics()
        {
            var first = _transactionRepository.getFirst();

            return View();
        }

        public IActionResult AnalyticsBalance()
        {
            return View();
        }

        public IActionResult AnalyticsExpenses()
        {
            return View();
        }

        public IActionResult AnalyticsIncome()
        {
            return View();
        }

        public IActionResult AnalyticsIncomeVsExpenses()
        {
            return View();
        }

        public IActionResult AnalyticsTransactionHistory()
        {
            return View();
        }
    }
}
