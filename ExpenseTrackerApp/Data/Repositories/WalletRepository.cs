using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;

namespace ExpenseTrackerApp.Data.Repositories;

public class WalletRepository : Repository<Wallet>, IWalletRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ITransactionRepository _transactionRepository;

    public WalletRepository(ApplicationDbContext applicationDbContext,
        ITransactionRepository transactionRepository) : base(applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
        _transactionRepository = transactionRepository;
    }
}