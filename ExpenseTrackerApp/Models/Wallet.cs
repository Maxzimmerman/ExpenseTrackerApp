using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrackerApp.Models;

public class Wallet
{
    [Key]
    public int Id { get; set; }
    public string BankName { get; set; }
    public string AccountName { get; set; }
    public string CreditCardNumber { get; set; }
    public decimal PersonalFunds { get; set; }
    public decimal CreditLimits { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; }
    public string AccessToken { get; set; }
    public string ItemId { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    
    [ForeignKey("ApplicationUser")]
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}