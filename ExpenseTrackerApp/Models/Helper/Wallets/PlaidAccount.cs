namespace ExpenseTrackerApp.Models.Helper.Wallets;

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