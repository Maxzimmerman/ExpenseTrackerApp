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

        [HttpGet]
        public IActionResult Analytics()
        {
            var first = _transactionRepository.getFirst();

            return View();
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
