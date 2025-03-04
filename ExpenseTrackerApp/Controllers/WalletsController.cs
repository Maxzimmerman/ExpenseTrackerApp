using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Services.IServices;

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
        if (data == null || string.IsNullOrEmpty(data.public_token))
        {
            return BadRequest(new { error = "Invalid request payload" });
        }

        _logger.LogInformation($"Exchanging public token: {data.public_token}");

        var requestBody = new
        {
            client_id = clientId,
            secret = secret,
            public_token = data.public_token
        };

        using var client = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync($"{baseUrl}/item/public_token/exchange", content);
        string responseBody = await response.Content.ReadAsStringAsync();

        _logger.LogInformation($"Plaid Exchange Response: {responseBody}");

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, new { error = responseBody });
        }

        using JsonDocument doc = JsonDocument.Parse(responseBody);
        string accessToken = doc.RootElement.GetProperty("access_token").GetString();
        string itemId = doc.RootElement.GetProperty("item_id").GetString();

        // Get user ID (assuming authentication)
        var user = _userRepository.getUserById(_userManagerSerive.GetCurrentUserId(User));
        
        if (user == null)
        {
            return Unauthorized();
        }

        // Get Institution Name (optional, helps user recognize their bank)
        var institutionResponse = await client.PostAsync($"{baseUrl}/institutions/get_by_id", 
            new StringContent(JsonSerializer.Serialize(new { client_id = clientId, secret = secret, institution_id = institutionId }), Encoding.UTF8, "application/json"));

        string bankName = "Unknown";
        string accountName = "Unknown";
        string creditCardNumber = "Unknown";
        decimal personalFunds = 0;
        decimal creditLimits = 0;
        decimal balance = 0;
        string currency = "Unknown";

        if (institutionResponse.IsSuccessStatusCode)
        {
            using JsonDocument institutionDoc = JsonDocument.Parse(await institutionResponse.Content.ReadAsStringAsync());
        }

        // Save or update wallet
        var existingWallet = _context.wallets.FirstOrDefault(w => w.ItemId == itemId && w.ApplicationUserId == user.Id);

        if (existingWallet == null)
        {
            _context.wallets.Add(new Wallet
            {
                BankName = bankName,
                AccountName = accountName,
                CreditCardNumber = creditCardNumber,
                PersonalFunds = personalFunds,  
                CreditLimits = creditLimits,
                Balance = balance,
                Currency = currency,
                AccessToken = accessToken,
                ItemId = itemId,
                LastUpdated = DateTime.Now,
                ApplicationUserId = user.Id,
            });
            await _context.SaveChangesAsync();
        }

        return new JsonResult(new { access_token = accessToken });
    }
    
    [HttpPost]
    public async Task<IActionResult> GetUserAccountInfo([FromBody] AccessTokenRequest data)
    {
        if (data == null || string.IsNullOrEmpty(data.access_token))
        {
            return BadRequest(new { error = "Invalid request payload" });
        }

        _logger.LogInformation("Fetching user account info...");

        var requestBody = new
        {
            client_id = clientId,
            secret = secret,
            access_token = data.access_token
        };

        string json = JsonSerializer.Serialize(requestBody);
        using var client = new HttpClient();
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync($"{baseUrl}/accounts/get", content);
        string responseBody = await response.Content.ReadAsStringAsync();

        _logger.LogInformation($"Plaid Accounts Response: {responseBody}");

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, new { error = responseBody });
        }

        using JsonDocument doc = JsonDocument.Parse(responseBody);
        var accounts = doc.RootElement.GetProperty("accounts")
            .EnumerateArray()
            .Select(account => new
            {
                Name = account.GetProperty("name").GetString(),
                Balance = account.GetProperty("balances").GetProperty("available").GetDecimal(),
                Currency = account.GetProperty("balances").GetProperty("iso_currency_code").GetString(),
                Type = account.GetProperty("type").GetString()
            })
            .ToList();

        return new JsonResult(accounts);
    }

    public class AccessTokenRequest
    {
        public string access_token { get; set; }
    }
}