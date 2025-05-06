using ExpenseTrackerApp.Models.Helper.Wallets;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerApp.Services.IServices;

public interface IPlaidService
{
    string createJsonRequestForPublicToken(string clientId, string clientSecret);
    Task<string> sendJsonRequestForPublicToken(string jsonRequest, string baseUrl);
    string transFormJsonResponseForPublicToken(string jsonRequest);
    Task<string> getPublicToken(string clientId, string clientSecret);
    Task<AccessToken> exchangingPublicForAccessToken(PublicTokenRequest request, string clientId, string clientSecret, string baseUrl);

    Task<PlaidAccountResponse> syncAccountData(AccessToken token, string clientId, string clientSecret, string baseUrl);
    Task<PlaidTransactionResponse> syncTransactions(AccessToken token, string clientId, string clientSecret, string baseUrl);
}