using SimpleBankingSystem.Enums;

namespace SimpleBankingSystem.Models;

/// <summary>
/// Rekening Giro (Current Account) yang mendukung penarikan melebihi saldo hingga batas limit cerukan (Overdraft).
/// Menerapkan Inheritance dari Account dan Polimorfisme pada CanWithdraw.
/// </summary>
public class CurrentAccount : Account
{
    public override AccountType Type => AccountType.Current;

    /// <summary>
    /// Batas limit cerukan (overdraft limit). Saldo dapat menjadi negatif hingga -OverdraftLimit.
    /// </summary>
    public decimal OverdraftLimit { get; set; } = 1_000_000m;

    public CurrentAccount(string accountNumber, Customer owner, decimal initialDeposit, decimal overdraftLimit = 1_000_000m)
        : base(accountNumber, owner, initialDeposit)
    {
        OverdraftLimit = overdraftLimit;
    }

    /// <summary>
    /// Validasi saldo untuk rekening giro: saldo setelah penarikan tidak boleh melebihi batas limit cerukan.
    /// </summary>
    protected override bool CanWithdraw(decimal amount, out string errorMessage)
    {
        if (Balance - amount < -OverdraftLimit)
        {
            decimal maxWithdrawable = Balance + OverdraftLimit;
            errorMessage = $"Penarikan gagal. Melebihi limit overdraft Rp {OverdraftLimit:N0}. Maksimum dana yang dapat ditarik saat ini: Rp {maxWithdrawable:N0}.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}
