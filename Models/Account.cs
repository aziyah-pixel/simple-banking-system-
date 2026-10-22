using SimpleBankingSystem.Enums;

namespace SimpleBankingSystem.Models;

/// <summary>
/// Kelas abstrak dasar untuk rekening bank (Menerapkan Abstraction & Encapsulation).
/// </summary>
public abstract class Account
{
    public string AccountNumber { get; }
    public Customer Owner { get; }
    public decimal Balance { get; protected set; }
    public DateTime CreatedAt { get; }
    public abstract AccountType Type { get; }

    private readonly List<Transaction> _transactions = new();
    public IReadOnlyList<Transaction> Transactions => _transactions.AsReadOnly();

    protected Account(string accountNumber, Customer owner, decimal initialDeposit)
    {
        AccountNumber = accountNumber;
        Owner = owner;
        CreatedAt = DateTime.Now;

        if (initialDeposit > 0)
        {
            Balance = initialDeposit;
            _transactions.Add(new Transaction(
                TransactionType.Deposit,
                initialDeposit,
                Balance,
                "Setoran Awal Pembukaan Rekening"
            ));
        }
    }

    /// <summary>
    /// Menyetor sejumlah dana ke rekening.
    /// </summary>
    public virtual bool Deposit(decimal amount, string description = "Setor Tunai")
    {
        if (amount <= 0)
        {
            return false;
        }

        Balance += amount;
        _transactions.Add(new Transaction(
            TransactionType.Deposit,
            amount,
            Balance,
            description
        ));
        return true;
    }

    /// <summary>
    /// Validasi apakah penarikan dana diizinkan (Diimplementasikan secara polimorfik oleh turunan).
    /// </summary>
    protected abstract bool CanWithdraw(decimal amount, out string errorMessage);

    /// <summary>
    /// Melakukan penarikan dana tunai.
    /// </summary>
    public virtual bool Withdraw(decimal amount, string description, out string errorMessage)
    {
        if (amount <= 0)
        {
            errorMessage = "Nominal penarikan harus lebih besar dari 0.";
            return false;
        }

        if (!CanWithdraw(amount, out errorMessage))
        {
            return false;
        }

        Balance -= amount;
        _transactions.Add(new Transaction(
            TransactionType.Withdrawal,
            amount,
            Balance,
            description
        ));
        return true;
    }

    /// <summary>
    /// Menerima transfer dana dari rekening lain.
    /// </summary>
    public virtual void ReceiveTransfer(decimal amount, string fromAccountNumber, string note)
    {
        Balance += amount;
        _transactions.Add(new Transaction(
            TransactionType.TransferIn,
            amount,
            Balance,
            $"Transfer Masuk dari Rek. {fromAccountNumber}: {note}"
        ));
    }

    /// <summary>
    /// Melakukan transfer dana ke rekening tujuan.
    /// </summary>
    public virtual bool TransferTo(Account destinationAccount, decimal amount, string note, out string errorMessage)
    {
        if (destinationAccount == null)
        {
            errorMessage = "Rekening tujuan tidak ditemukan.";
            return false;
        }

        if (destinationAccount.AccountNumber == this.AccountNumber)
        {
            errorMessage = "Tidak dapat mentransfer ke nomor rekening Anda sendiri.";
            return false;
        }

        if (amount <= 0)
        {
            errorMessage = "Nominal transfer harus lebih besar dari 0.";
            return false;
        }

        if (!CanWithdraw(amount, out errorMessage))
        {
            return false;
        }

        Balance -= amount;
        _transactions.Add(new Transaction(
            TransactionType.TransferOut,
            amount,
            Balance,
            $"Transfer Keluar ke Rek. {destinationAccount.AccountNumber} ({destinationAccount.Owner.FullName}): {note}"
        ));

        destinationAccount.ReceiveTransfer(amount, this.AccountNumber, note);
        return true;
    }
}
