using System.Text;
using System.Text.Json;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.Helper.Wallets;
using ExpenseTrackerApp.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerApp.Services;

public class PlaidService : IPlaidService
{
    public string createJsonRequestForPublicToken(string clientId, string clientSecret)
    {
        var requestBody = new
        {
            client_id = clientId,
            secret = clientSecret,
            user = new { client_user_id = "unique_user_id" },
            client_name = "YourAppName",
            products = new[] { "transactions" },
            country_codes = new[] { "US" },
            language = "en"
        };
        
        string json = JsonSerializer.Serialize(requestBody);
        Console.WriteLine($"Json Request{json}");
        return json;
    }

    public async Task<string> sendJsonRequestForPublicToken(string jsonRequest, string baseUrl)
    {
        using (var client = new HttpClient())
        {
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync($"{baseUrl}/link/token/create", content);
            string responseBody = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error sending request");
            }
            Console.WriteLine($"Json Response {responseBody}");
            return responseBody;   
        }
    }

    public string transFormJsonResponseForPublicToken(string jsonResponse)
    {
        using JsonDocument doc = JsonDocument.Parse(jsonResponse);
        string linkToken = doc.RootElement.GetProperty("link_token").GetString();
        Console.WriteLine($"Link Token: {linkToken}");
        return linkToken;
    }

    public async Task<string> getPublicToken(string clientId, string clientSecret, string baseUrl)
    {
        string requestJson = createJsonRequestForPublicToken(clientId, clientSecret);
        string responseJson = await sendJsonRequestForPublicToken(requestJson, baseUrl);
        string linkToken = transFormJsonResponseForPublicToken(responseJson);
        return linkToken;
    }

    public async Task<AccessToken> exchangingPublicForAccessToken(PublicTokenRequest request, string clientId, string clientSecret, string baseUrl)
    {
        // Exchange public token for access token
        var requestBody = new
        {
            client_id = clientId,
            secret = clientSecret,
            public_token = request.public_token
        };

        using var client = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        HttpResponseMessage exchangeResponse = await client.PostAsync($"{baseUrl}/item/public_token/exchange", content);
        string exchangeResponseBody = await exchangeResponse.Content.ReadAsStringAsync();

        var exchangeData = JsonSerializer.Deserialize<PlaidExchangeResponse>(exchangeResponseBody);
        string accessToken = exchangeData.access_token;
        string itemId = exchangeData.item_id;
        return new AccessToken() { Token = accessToken, ItemId = itemId };
    }

    public async Task<PlaidAccountResponse> syncAccountData(AccessToken token, string clientId, string clientSecret, string baseUrl)
    {
        var accountsRequestBody = new
        {
            client_id = clientId,
            secret = clientSecret,
            access_token = token.Token
        };

        using var client = new HttpClient();
        var accountsContent = new StringContent(JsonSerializer.Serialize(accountsRequestBody), Encoding.UTF8, "application/json");
        HttpResponseMessage accountsResponse = await client.PostAsync($"{baseUrl}/accounts/get", accountsContent);
        string accountsResponseBody = await accountsResponse.Content.ReadAsStringAsync();

        var accountsData = JsonSerializer.Deserialize<PlaidAccountResponse>(accountsResponseBody);
        return accountsData;
    }

    public async Task<PlaidTransactionResponse> syncTransactions(AccessToken token, string clientId, string clientSecret, string baseUrl)
    {
        // Fetch transactions
        var transactionsRequestBody = new
        {
            client_id = clientId,
            secret = clientSecret,
            access_token = token.Token,
            start_date = DateTime.UtcNow.AddMonths(-1).ToString("yyyy-MM-dd"),
            end_date = DateTime.UtcNow.ToString("yyyy-MM-dd")
        };

        var client = new HttpClient();
        var transactionsContent = new StringContent(JsonSerializer.Serialize(transactionsRequestBody), Encoding.UTF8, "application/json");
        HttpResponseMessage transactionsResponse = await client.PostAsync($"{baseUrl}/transactions/get", transactionsContent);
        string transactionsResponseBody = await transactionsResponse.Content.ReadAsStringAsync();
        
        //Console.WriteLine($" --------------- Plaid Transactions Response: {transactionsResponseBody}");
        
        if (!transactionsResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"{(int)transactionsResponse.StatusCode} {new { error = transactionsResponse }}");
        }
        
        var transactionsData = JsonSerializer.Deserialize<PlaidTransactionResponse>(transactionsResponseBody);
        
        if (transactionsData == null)
            throw new Exception("Error sending request for fetching transactions");
        
        return transactionsData;
    }

}