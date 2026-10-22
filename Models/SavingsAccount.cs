using SimpleBankingSystem.Enums;

namespace SimpleBankingSystem.Models;

/// <summary>
/// Rekening Tabungan (Savings Account) dengan batasan saldo minimum mengendap.
/// Menerapkan Inheritance dari Account dan Polimorfisme pada CanWithdraw.
/// </summary>
public class SavingsAccount : Account
{
    public override AccountType Type => AccountType.Savings;

    /// <summary>
    /// Batas saldo minimum yang harus mengendap di rekening.
    /// </summary>
    public decimal MinimumBalance { get; set; } = 50_000m;

    public SavingsAccount(string accountNumber, Customer owner, decimal initialDeposit, decimal minimumBalance = 50_000m)
        : base(accountNumber, owner, initialDeposit)
    {
        MinimumBalance = minimumBalance;
    }

    /// <summary>
    /// Validasi saldo untuk rekening tabungan: saldo setelah penarikan tidak boleh di bawah MinimumBalance.
    /// </summary>
    protected override bool CanWithdraw(decimal amount, out string errorMessage)
    {
        if (Balance - amount < MinimumBalance)
        {
            errorMessage = $"Penarikan gagal. Rekening Tabungan wajib menyisakan saldo minimum Rp {MinimumBalance:N0}. (Saldo Anda saat ini: Rp {Balance:N0})";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}
