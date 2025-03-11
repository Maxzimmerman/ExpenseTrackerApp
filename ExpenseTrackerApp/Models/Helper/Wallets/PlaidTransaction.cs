namespace ExpenseTrackerApp.Models.Helper.Wallets;

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