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

    public WalletsController(IFooterRepository footerRepository) : base(footerRepository)
    {
    }

    [HttpGet]
    public IActionResult Wallets()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> CreatePublicLink()
    {
        _logger.LogInformation("Creating a Plaid public link...");

        try
        {
            using (HttpClient client = new HttpClient())
            {
                // Prepare the request body with necessary details for creating a public link
                var requestBody = new
                {
                    client_id = clientId,
                    secret = secret,
                    institution_id = institutionId,
                    initial_products = new[] { "transactions" }
                };

                // Serialize the body to JSON format
                string json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Send the POST request to Plaid API
                HttpResponseMessage response =
                    await client.PostAsync($"{baseUrl}/sandbox/public_token/create", content);

                // If the response is not successful, handle the error
                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { error = errorMessage });
                }

                // Parse the response body to extract the public_token
                string responseBody = await response.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(responseBody))
                {
                    if (doc.RootElement.TryGetProperty("public_token", out var publicToken))
                    {
                        // Return the public token as a JSON response
                        return Ok(new { link_token = publicToken.GetString() });
                    }
                    else
                    {
                        // If the public_token is not found in the response, return an error
                        return StatusCode(500, new { error = "Public token not found in response" });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log and return the exception message if an error occurs
            _logger.LogError($"Error creating public link: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}