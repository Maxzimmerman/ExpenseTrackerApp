using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Services.IServices;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackerApp.Models.Helper.Wallets;

namespace ExpenseTrackerApp.Controllers;

[Authorize]
public class WalletsController : BaseController
{
    private readonly ILogger<WalletsController> _logger;
    private readonly IFooterRepository _footerRepository;
    private readonly ApplicationDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUserManageService _userManageService;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPlaidService _plaidService;
    private readonly string clientId = "6748bb5fc33b04001a5b72eb";
    private readonly string secret = "70581998b11cf373c0f52d6950c067";
    private readonly string baseUrl = "https://sandbox.plaid.com"; // Use sandbox environment
    private readonly string institutionId = "ins_109508"; // Example test institution

    public WalletsController(ILogger<WalletsController> logger, 
        IFooterRepository footerRepository, ApplicationDbContext applicationDbContext,
        IUserRepository userRepository, IUserManageService userManageService, 
        IPlaidService plaidService, IWalletRepository walletRepository,
        ITransactionRepository transactionRepository, ICategoryRepository categoryRepository) : base(footerRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userManageService = userManageService ?? throw new ArgumentNullException(nameof(userManageService));
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _plaidService = plaidService ?? throw new ArgumentNullException(nameof(plaidService));
    }

    [HttpGet]
    public IActionResult Wallets()
    {
        var user = _userRepository.getUserById(_userManageService.GetCurrentUserId(User));
        
        var wallets = _walletRepository.getAllByUser(user.Id);

        // Ensure Transactions is always initialized
        // Todo
        foreach (var wallet in wallets)
        {
            wallet.Transactions = _context.transactions
                .Include(t => t.Category)
                .Include(t => t.Category.CategoryType)
                .Include(t => t.Category.CategoryColor)
                .Include(t => t.Category.CategoryIcon)
                .OrderByDescending(t => t.Date)
                .Where(w => w.WalletId == wallet.Id).ToList();
        }

        return View(wallets);
    }

    [HttpGet]
    public async Task<IActionResult> CreatePublicLink()
    {
        try
        {
            _logger.LogInformation("Creating a Plaid public link...");
            string linkToken = await _plaidService.getPublicToken(clientId, secret, baseUrl);
            
            return new JsonResult(new { link_token = linkToken });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in CreatePublicLink: {ex.Message}");
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ExchangePublicToken([FromBody] PublicTokenRequest data)
    {
        AccessToken exchangeData = await _plaidService.exchangingPublicForAccessToken(data, clientId, secret, baseUrl);
        string accessToken = exchangeData.Token;
        string itemId = exchangeData.ItemId;

        // Fetch Accounts Data
        PlaidAccountResponse accountsData = await _plaidService.syncAccountData(exchangeData, clientId, secret, baseUrl);

        var firstAccount = accountsData.accounts[0];
        string bankName = firstAccount.name;
        string accountId = firstAccount.account_id;
        string accountName = firstAccount.official_name ?? "Unknown";
        decimal balance = firstAccount.balances.current;
        decimal personalFunds = firstAccount.balances.available ?? 0;
        decimal creditLimits = firstAccount.balances.limit ?? 0;
        string currency = firstAccount.balances.iso_currency_code;

        var user = _userRepository.getUserById(_userManageService.GetCurrentUserId(User));
        if (user == null)
        {
            return Unauthorized();
        }

        // Save or update wallet
        var existingWallet = _walletRepository.getByUserAndBankName(user.Id, bankName);

        if (existingWallet == null)
        {
            existingWallet = new Wallet
            {
                BankName = bankName,
                AccountName = accountName,
                CreditCardNumber = "Unknown",
                PersonalFunds = personalFunds,
                CreditLimits = creditLimits,
                Balance = balance,
                Currency = currency,
                AccessToken = accessToken,
                ItemId = itemId,
                LastUpdated = DateTime.UtcNow,
                ApplicationUserId = user.Id,
            };
            _walletRepository.Add(existingWallet);
        }
        _logger.LogInformation("Created Wallet Successfully");

        // Fetch transactions
        var transactionsData = await _plaidService.syncTransactions(exchangeData, clientId, secret, baseUrl);

        // Fetch existing transaction IDs to prevent duplicates
        var existingTransactionIds = _transactionRepository.getIdsForWallet(user.Id, existingWallet.Id);
        
        // Get Expense and Income default Category
        var incomeCategoryId = _categoryRepository.getIncomeDefaultCategoryId(User);
        var expenseCategoryId = _categoryRepository.getExpenseDefaultCategoryId(User);
        
        if (incomeCategoryId == null)
        {
            _logger.LogInformation("------------- no income default category");
            throw new Exception("No income default category");
        }
        
        if (expenseCategoryId == null)
        {
            _logger.LogInformation("------------- no expense default category");
            throw new Exception("No expense default category");
        }

        // Filter transactions for the correct account and prevent duplicates
        var newExpenseTransactions = transactionsData.transactions
            .Where(t => t.account_id == accountId) // Ensure it's for the added account
            .Where(t => t.amount < 0)
            .Where(t => !existingTransactionIds.Contains(t.transaction_id)) // Avoid duplicates
            .Select(t => new Transaction
            {
                Title = t.name,
                Description = t.merchant_name ?? "Unknown",
                Date = DateTime.SpecifyKind(DateTime.Parse(t.date), DateTimeKind.Utc),
                Amount = t.amount * -1, // ensure negative values always are positive
                TransactionId = t.transaction_id,
                CategoryId = expenseCategoryId,
                ApplicationUserId = user.Id,
                WalletId = existingWallet.Id
            })
            .ToList();
        
        var newIncomeTransactions = transactionsData.transactions
            .Where(t => t.account_id == accountId) // Ensure it's for the added account
            .Where(t => t.amount > 0)
            .Where(t => !existingTransactionIds.Contains(t.transaction_id)) // Avoid duplicates
            .Select(t => new Transaction
            {
                Title = t.name,
                Description = t.merchant_name ?? "Unknown",
                Date = DateTime.SpecifyKind(DateTime.Parse(t.date), DateTimeKind.Utc),
                Amount = t.amount,
                TransactionId = t.transaction_id,
                CategoryId = incomeCategoryId,
                ApplicationUserId = user.Id,
                WalletId = existingWallet.Id
            })
            .ToList();

        if (!newExpenseTransactions.Any())
        {
            _logger.LogInformation("No new transactions to add.");
            return Ok(new { message = "No new transactions found" });
        }
        
        if (!newIncomeTransactions.Any())
        {
            _logger.LogInformation("No new transactions to add.");
            return Ok(new { message = "No new transactions found" });
        }

        // Save new transactions
        _transactionRepository.addTransactions(newExpenseTransactions);
        _transactionRepository.addTransactions(newIncomeTransactions);

        _logger.LogInformation($" ------ Added expenses {newExpenseTransactions.Count} and income {newIncomeTransactions} new transactions for Wallet {existingWallet.Id}");

        return new JsonResult(new { access_token = accessToken });
    }
}