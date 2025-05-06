using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using Microsoft.EntityFrameworkCore;

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

    public Wallet getByUserAndBankName(string userId, string bankName)
    {
        return _applicationDbContext.wallets.FirstOrDefault(w => w.ApplicationUserId == userId && w.BankName == bankName);    
    }

    public void add(Wallet wallet)
    {
        _applicationDbContext.wallets.Add(wallet);
        _applicationDbContext.SaveChanges();
    }

    public List<Wallet> getAllByUser(string userId)
    {
        return _applicationDbContext.wallets
            .Include(w => w.Transactions)
            .Where(w => w.ApplicationUserId == userId)
            .ToList();
    }
}