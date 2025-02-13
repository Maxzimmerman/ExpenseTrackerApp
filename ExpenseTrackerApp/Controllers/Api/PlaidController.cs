using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerApp.Controllers.Api;

[Route("Api/[controller]")]
[ApiController]
public class PlaidController : ControllerBase
{
    private readonly ILogger<PlaidController> _logger;
    private readonly string clientId = "6748bb5fc33b04001a5b72eb"; // Replace with your Plaid client_id
    private readonly string secret = "70581998b11cf373c0f52d6950c067"; // Replace with your Plaid secret
    private readonly string baseUrl = "https://sandbox.plaid.com"; // Use sandbox environment
    private readonly string institutionId = "ins_109508"; // Example test institution

    public PlaidController(ILogger<PlaidController> logger)
    {
        _logger = logger;
    }

    [HttpGet("public-link")]
    public async Task<IActionResult> CreatePublicLink()
    {
        _logger.LogInformation("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!Creating a link!!!!!!!!!!!!!!!!!!!!!!!");
        try
        {
            using HttpClient client = new HttpClient();

            var requestBody = new
            {
                client_id = clientId,
                secret = secret,
                institution_id = institutionId,
                initial_products = new[] { "transactions" }
            };

            string json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync($"{baseUrl}/sandbox/public_token/create", content);

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { error = errorMessage });
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(responseBody);
            string publicToken = doc.RootElement.GetProperty("public_token").GetString();

            return Ok(new { public_token = publicToken });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}