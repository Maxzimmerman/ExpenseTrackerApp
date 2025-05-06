using ExpenseTrackerApp.Models;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories;

public interface IWalletRepository : IRepository<Wallet>
{
    Wallet getByUserAndBankName(string userId, string bankName);
    void add(Wallet wallet);
    List<Wallet> getAllByUser(string userId);
}