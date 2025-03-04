using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Models;

namespace ExpenseTrackerApp.SeedDataBase;

public class SeedWallet
{
    private readonly ApplicationDbContext _context;

    public SeedWallet(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public void Seed()
    {
        List<Wallet> wallets = new List<Wallet>()
        {
            new Wallet()
            {
                BankName = "Sparkasse",
                AccountName = "Max Zimmermann",
                CreditCardNumber = "987654321",
                PersonalFunds = 40,
                CreditLimits = 1000,
                Balance = 100,
                Currency = "USD",
                AccessToken = "test",
                LastUpdated = DateTime.UtcNow,
                ApplicationUser = new ApplicationUser(),
            },
            new Wallet()
            {
                BankName = "Ing",
                AccountName = "Max Zimmermann",
                CreditCardNumber = "32930423",
                PersonalFunds = 40,
                CreditLimits = 1000,
                Balance = 100,
                Currency = "USD",
                AccessToken = "test",
                LastUpdated = DateTime.UtcNow,
                ApplicationUser = new ApplicationUser(),
            }
        };
        
        _context.AddRange(wallets);
        _context.SaveChanges();
    }
}