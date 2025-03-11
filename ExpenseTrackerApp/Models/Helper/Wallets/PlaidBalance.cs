namespace ExpenseTrackerApp.Models.Helper.Wallets;

public class PlaidBalance
{
    public decimal? available { get; set; }
    public decimal current { get; set; }
    public decimal? limit { get; set; }
    public string iso_currency_code { get; set; }
}