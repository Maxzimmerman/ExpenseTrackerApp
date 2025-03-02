using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace ExpenseTrackerApp.Controllers;

[Authorize]
public class WalletsController : BaseController
{
    private readonly ILogger<WalletsController> _logger;
    private readonly IFooterRepository _footerRepository;
    private readonly string clientId = "6748bb5fc33b04001a5b72eb"; // Replace with your Plaid client_id
    private readonly string secret = "70581998b11cf373c0f52d6950c067"; // Replace with your Plaid secret
    private readonly string baseUrl = "https://sandbox.plaid.com"; // Use sandbox environment
    private readonly string institutionId = "ins_109508"; // Example test institution

    public WalletsController(ILogger<WalletsController> logger, 
        IFooterRepository footerRepository) : base(footerRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public IActionResult Wallets()
    {
        return View();
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

        string json = JsonSerializer.Serialize(requestBody);
        using var client = new HttpClient();
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync($"{baseUrl}/item/public_token/exchange", content);
        string responseBody = await response.Content.ReadAsStringAsync();

        _logger.LogInformation($"Plaid Exchange Response: {responseBody}");

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, new { error = responseBody });
        }

        using JsonDocument doc = JsonDocument.Parse(responseBody);
        string accessToken = doc.RootElement.GetProperty("access_token").GetString();

        _logger.LogInformation("Access token retrieved successfully.");
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