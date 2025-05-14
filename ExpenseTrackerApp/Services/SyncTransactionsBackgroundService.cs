using Microsoft.Extensions.Hosting;
using System;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.Helper.Wallets;
using ExpenseTrackerApp.Services.IServices;

namespace ExpenseTrackerApp.Services;

public class SyncTransactionsBackgroundService : IHostedService, IDisposable
{
    private Timer? _timer;
    private readonly ILogger<SyncTransactionsBackgroundService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private string _publicToken;
    private readonly string clientId = "6748bb5fc33b04001a5b72eb";
    private readonly string secret = "70581998b11cf373c0f52d6950c067";
    private readonly string baseUrl = "https://sandbox.plaid.com";
    private readonly IHttpContextAccessor _httpContextAccessor; 

    public SyncTransactionsBackgroundService(
        ILogger<SyncTransactionsBackgroundService> logger, 
        IServiceScopeFactory serviceScopeFactory,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _scopeFactory = serviceScopeFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Start the timer with an initial delay of 0, then repeat every 10 minutes.
        _timer = new Timer(async _ => await SyncTransactions(cancellationToken), null, TimeSpan.Zero,
            TimeSpan.FromMinutes(60));
        return Task.CompletedTask;
    }

    private async Task SyncTransactions(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"SyncTransactionsBackgroundService is starting... {DateTime.Now.ToShortTimeString()}");
        
        using var scope = _scopeFactory.CreateScope();
        var plaidService = scope.ServiceProvider.GetRequiredService<IPlaidService>();
        var walletRepository = scope.ServiceProvider.GetRequiredService<IWalletRepository>();
        var transactionRepository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
        var categoryRepository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();

        // Fetch all wallets.
        List<Wallet> wallets = walletRepository.getAll();
        if (wallets.Count == 0)
        {
            _logger.LogInformation($"No Wallet to sync. wallets: {wallets.Count}");
        }
        foreach (var wallet in wallets)
        {
            try
            {
                // Use the access token associated with the wallet
                var accessToken = new AccessToken { Token = wallet.AccessToken };

                _logger.LogInformation($"Syncing transactions for Wallet: {wallet.Id}");

                // Fetch transactions using the stored access token.
                var transactionsData = await plaidService.syncTransactions(accessToken, clientId, secret, baseUrl);

                // Get existing transaction IDs for the wallet to prevent duplicates.
                var existingTransactionIds = transactionRepository.getIdsForWallet(wallet.ApplicationUserId, wallet.Id);

                // Get default categories (Income and Expense)
                var incomeCategoryId = categoryRepository.getIncomeDefaultCategoryId();
                var expenseCategoryId = categoryRepository.getExpenseDefaultCategoryId();

                if (incomeCategoryId == null || expenseCategoryId == null)
                {
                    _logger.LogError("Missing Income or Expense Default Categories.");
                    continue; // Skip syncing for this wallet if categories are missing.
                }

                // Filter and map new transactions for income and expenses
                var newExpenseTransactions = transactionsData.transactions
                    .Where(t => t.account_id == wallet.ApplicationUserId && t.amount < 0 &&
                                !existingTransactionIds.Contains(t.transaction_id))
                    .Select(t => new Models.Transaction
                    {
                        Title = t.name,
                        Description = t.merchant_name ?? "Unknown",
                        Date = DateTime.Parse(t.date).ToUniversalTime(),
                        Amount = t.amount * -1, // Expense amounts are negative, make them positive
                        TransactionId = t.transaction_id,
                        CategoryId = expenseCategoryId,
                        WalletId = wallet.Id,
                        ApplicationUserId = wallet.ApplicationUserId
                    }).ToList();

                var newIncomeTransactions = transactionsData.transactions
                    .Where(t => t.account_id == wallet.ApplicationUserId && t.amount > 0 &&
                                !existingTransactionIds.Contains(t.transaction_id))
                    .Select(t => new Models.Transaction
                    {
                        Title = t.name,
                        Description = t.merchant_name ?? "Unknown",
                        Date = DateTime.Parse(t.date).ToUniversalTime(),
                        Amount = t.amount,
                        TransactionId = t.transaction_id,
                        CategoryId = incomeCategoryId,
                        WalletId = wallet.Id,
                        ApplicationUserId = wallet.ApplicationUserId
                    }).ToList();

                // Save new transactions to the database
                if (newExpenseTransactions.Any())
                {
                    transactionRepository.addTransactions(newExpenseTransactions);
                }

                if (newIncomeTransactions.Any())
                {
                    transactionRepository.addTransactions(newIncomeTransactions);
                }

                _logger.LogInformation(
                    $"Wallet {wallet.Id}: Added {newExpenseTransactions.Count} expenses and {newIncomeTransactions.Count} income transactions.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to sync wallet {wallet.Id}: {ex.Message}");
            }
        }
        
        _logger.LogInformation($"SyncTransactionsBackgroundService is stopping... {DateTime.Now.ToShortTimeString()}");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}