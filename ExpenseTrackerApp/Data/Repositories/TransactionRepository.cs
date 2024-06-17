using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.TransactionViewModels;

namespace ExpenseTrackerApp.Data.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public ExpenseTrackerApp.Models.Transaction getFirst()
        {
            return _context.transactions
                .Include(t => t.Category)
                .Include(t => t.Category.CategoryType)
                .Include(t => t.Category.CategoryColor)
                .Include(t => t.Category.CategoryIcon)
                .FirstOrDefault(t => t.Id == 3);

        }

        public AnalyticsData GetAnalyticsData(string userId)
        {
            int transactions = _context.transactions.ToList().Count;
            int categories = _context.categories.ToList().Count;
            AnalyticsData data = new AnalyticsData(transactions, categories);
            return data;
        }
    }
}
