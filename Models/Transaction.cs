using SimpleBankingSystem.Enums;

namespace SimpleBankingSystem.Models;

/// <summary>
/// Merepresentasikan riwayat mutasi keuangan akun.
/// </summary>
public class Transaction
{
    public string Id { get; }
    public DateTime Timestamp { get; }
    public TransactionType Type { get; }
    public decimal Amount { get; }
    public decimal BalanceAfter { get; }
    public string Description { get; }

    public Transaction(TransactionType type, decimal amount, decimal balanceAfter, string description)
    {
        Id = "TXN-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        Timestamp = DateTime.Now;
        Type = type;
        Amount = amount;
        BalanceAfter = balanceAfter;
        Description = description;
    }
}
