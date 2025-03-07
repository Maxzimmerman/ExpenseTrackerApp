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

namespace ExpenseTrackerApp.Controllers;

[Authorize]
public class WalletsController : BaseController
{
    private readonly ILogger<WalletsController> _logger;
    private readonly IFooterRepository _footerRepository;
    private readonly ApplicationDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IUserManageService _userManagerSerive;
    private readonly string clientId = "6748bb5fc33b04001a5b72eb"; // Replace with your Plaid client_id
    private readonly string secret = "70581998b11cf373c0f52d6950c067"; // Replace with your Plaid secret
    private readonly string baseUrl = "https://sandbox.plaid.com"; // Use sandbox environment
    private readonly string institutionId = "ins_109508"; // Example test institution

    public WalletsController(ILogger<WalletsController> logger, 
        IFooterRepository footerRepository, ApplicationDbContext applicationDbContext,
        IUserRepository userRepository, IUserManageService userManageService) : base(footerRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userManagerSerive = userManageService ?? throw new ArgumentNullException(nameof(userManageService));
    }

    [HttpGet]
    public IActionResult Wallets()
    {
        var wallets = _context.wallets.ToList();
        return View(wallets);
    }

    [HttpGet]
    public async Task<IActionResult> CreatePublicLink()
    {
        try
        {
            _logger.LogInformation("Creating a Plaid public link...");

            var requestBody = new
            {
                client_id = clientId,
                secret = secret,
                user = new { client_user_id = "unique_user_id" },
                client_name = "YourAppName",
                products = new[] { "transactions" },
                country_codes = new[] { "US" },
                language = "en"
            };

            string json = JsonSerializer.Serialize(requestBody);
            using var client = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync($"{baseUrl}/link/token/create", content);
            string responseBody = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"Plaid API Response: {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new { error = responseBody });
            }

            using JsonDocument doc = JsonDocument.Parse(responseBody);
            string linkToken = doc.RootElement.GetProperty("link_token").GetString();

            _logger.LogInformation($"Generated link_token: {linkToken}");

            return new JsonResult(new { link_token = linkToken });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in CreatePublicLink: {ex.Message}");
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    }

    public class PublicTokenRequest
    {
        public string public_token { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> ExchangePublicToken([FromBody] PublicTokenRequest data)
    {
        // Validate request
        if (data == null || string.IsNullOrEmpty(data.public_token))
        {
            return BadRequest(new { error = "Invalid request payload" });
        }

        // Exchange public token for access token
        var requestBody = new
        {
            client_id = clientId,
            secret = secret,
            public_token = data.public_token
        };

        using var client = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        HttpResponseMessage exchangeResponse = await client.PostAsync($"{baseUrl}/item/public_token/exchange", content);
        string exchangeResponseBody = await exchangeResponse.Content.ReadAsStringAsync();

        if (!exchangeResponse.IsSuccessStatusCode)
        {
            return StatusCode((int)exchangeResponse.StatusCode, new { error = exchangeResponseBody });
        }

        var exchangeData = JsonSerializer.Deserialize<PlaidExchangeResponse>(exchangeResponseBody);
        string accessToken = exchangeData.access_token;
        string itemId = exchangeData.item_id;

        // Fetch account details
        var accountsRequestBody = new
        {
            client_id = clientId,
            secret = secret,
            access_token = accessToken
        };

        var accountsContent = new StringContent(JsonSerializer.Serialize(accountsRequestBody), Encoding.UTF8, "application/json");
        HttpResponseMessage accountsResponse = await client.PostAsync($"{baseUrl}/accounts/get", accountsContent);
        string accountsResponseBody = await accountsResponse.Content.ReadAsStringAsync();

        if (!accountsResponse.IsSuccessStatusCode)
        {
            return StatusCode((int)accountsResponse.StatusCode, new { error = accountsResponseBody });
        }

        var accountsData = JsonSerializer.Deserialize<PlaidAccountsResponse>(accountsResponseBody);

        if (accountsData.accounts == null || accountsData.accounts.Length == 0)
        {
            return BadRequest(new { error = "No accounts found in the response" });
        }

        var firstAccount = accountsData.accounts[0];
        string bankName = firstAccount.name;
        string accountId = firstAccount.account_id;
        string accountName = firstAccount.official_name ?? "Unknown";
        decimal balance = firstAccount.balances.current;
        decimal personalFunds = firstAccount.balances.available ?? 0;
        decimal creditLimits = firstAccount.balances.limit ?? 0;
        string currency = firstAccount.balances.iso_currency_code;

        var user = _userRepository.getUserById(_userManagerSerive.GetCurrentUserId(User));
        if (user == null)
        {
            return Unauthorized();
        }

        // Save or update wallet
        var existingWallet = _context.wallets.FirstOrDefault(w => w.ApplicationUserId == user.Id && w.BankName == bankName);

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

            _context.wallets.Add(existingWallet);
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation("Created Wallet Successfully");

        // Fetch transactions
        var transactionsRequestBody = new
        {
            client_id = clientId,
            secret = secret,
            access_token = accessToken,
            start_date = DateTime.UtcNow.AddMonths(-1).ToString("yyyy-MM-dd"),
            end_date = DateTime.UtcNow.ToString("yyyy-MM-dd")
        };

        var transactionsContent = new StringContent(JsonSerializer.Serialize(transactionsRequestBody), Encoding.UTF8, "application/json");
        HttpResponseMessage transactionsResponse = await client.PostAsync($"{baseUrl}/transactions/get", transactionsContent);
        string transactionsResponseBody = await transactionsResponse.Content.ReadAsStringAsync();

        // _logger.LogInformation($"Plaid Transactions Response: {transactionsResponseBody}");

        if (!transactionsResponse.IsSuccessStatusCode)
        {
            return StatusCode((int)transactionsResponse.StatusCode, new { error = transactionsResponseBody });
        }

        var transactionsData = JsonSerializer.Deserialize<PlaidTransactionsResponse>(transactionsResponseBody);

        // Fetch existing transaction IDs to prevent duplicates
        var existingTransactionIds = _context.transactions
            .Where(t => t.WalletId == existingWallet.Id)
            .Select(t => t.TransactionId)
            .ToList();
        
        // Get Expense and Income default Category
        var incomeCategoryId = _context.categories
            .Include(ici => ici.CategoryType)
            .FirstOrDefault(ici => ici.CategoryType.Name == "Income" && ici.Title == "Default").Id;
        
        var expenseCategoryId = _context.categories
            .Include(ici => ici.CategoryType)
            .FirstOrDefault(ici => ici.CategoryType.Name == "Expense" && ici.Title == "Default").Id;

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
                Amount = t.amount * -1, // Ensuere negative values always are positive
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
        await _context.transactions.AddRangeAsync(newExpenseTransactions);
        await _context.transactions.AddRangeAsync(newIncomeTransactions);
        await _context.SaveChangesAsync();

        _logger.LogInformation($" ------ Added expenses {newExpenseTransactions.Count} and income {newIncomeTransactions} new transactions for Wallet {existingWallet.Id}");

        return new JsonResult(new { access_token = accessToken });
    }

    
    public class PlaidTransactionsResponse
    {
        public PlaidTransaction[] transactions { get; set; }
    }
    
    public class PlaidTransaction
    {
        public string transaction_id { get; set; }
        public string name { get; set; }
        public decimal amount { get; set; }
        public string iso_currency_code { get; set; }
        public string date { get; set; }
        public string[] category { get; set; }
        public string account_id { get; set; }
        public string merchant_name { get; set; }
    }
    
    public class PlaidExchangeResponse
    {
        public string access_token { get; set; }
        public string item_id { get; set; }
    }

    public class PlaidAccountsResponse
    {
        public PlaidAccount[] accounts { get; set; }
    }

    public class PlaidAccount
    {
        public string account_id { get; set; }
        public string name { get; set; }
        public string official_name { get; set; }
        public string mask { get; set; }
        public string subtype { get; set; }
        public string type { get; set; }
        public PlaidBalance balances { get; set; }
        public List<Transaction> transactions { get; set; }
    }

    public class PlaidBalance
    {
        public decimal? available { get; set; }
        public decimal current { get; set; }
        public decimal? limit { get; set; }
        public string iso_currency_code { get; set; }
    }

    public class AccessTokenRequest
    {
        public string access_token { get; set; }
    }
}