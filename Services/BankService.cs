using SimpleBankingSystem.Enums;
using SimpleBankingSystem.Models;

namespace SimpleBankingSystem.Services;

/// <summary>
/// Layanan utama yang mengelola logika perbankan, akun nasabah, dan transaksi.
/// </summary>
public class BankService
{
    private readonly List<Account> _accounts = new();
    private int _nextSavingsId = 1001;
    private int _nextCurrentId = 2001;
    private int _nextCustomerId = 1;

    public BankService()
    {
        SeedSampleData();
    }

    /// <summary>
    /// Membuka rekening baru untuk nasabah.
    /// </summary>
    public Account OpenAccount(string fullName, string phoneNumber, string pin, AccountType type, decimal initialDeposit)
    {
        string customerId = $"CUST-{_nextCustomerId++:D4}";
        Customer customer = new Customer(customerId, fullName, phoneNumber, pin);

        Account account;
        if (type == AccountType.Savings)
        {
            string accNumber = $"SAV-{_nextSavingsId++}";
            account = new SavingsAccount(accNumber, customer, initialDeposit);
        }
        else
        {
            string accNumber = $"CUR-{_nextCurrentId++}";
            account = new CurrentAccount(accNumber, customer, initialDeposit);
        }

        _accounts.Add(account);
        return account;
    }

    /// <summary>
    /// Autentikasi nasabah berdasarkan nomor rekening dan PIN.
    /// </summary>
    public bool Authenticate(string accountNumber, string pin, out Account? account, out string errorMessage)
    {
        account = _accounts.FirstOrDefault(a => a.AccountNumber.Equals(accountNumber.Trim(), StringComparison.OrdinalIgnoreCase));
        if (account == null)
        {
            errorMessage = "Nomor rekening tidak ditemukan.";
            return false;
        }

        if (!account.Owner.VerifyPin(pin))
        {
            account = null;
            errorMessage = "PIN yang Anda masukkan salah.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    /// <summary>
    /// Mendapatkan detail akun berdasarkan nomor rekening.
    /// </summary>
    public Account? GetAccount(string accountNumber)
    {
        return _accounts.FirstOrDefault(a => a.AccountNumber.Equals(accountNumber.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Mengambil semua rekening (untuk referensi/overview).
    /// </summary>
    public IReadOnlyList<Account> GetAllAccounts()
    {
        return _accounts.AsReadOnly();
    }

    /// <summary>
    /// Menyetorkan dana ke nomor rekening.
    /// </summary>
    public bool Deposit(string accountNumber, decimal amount, out string message)
    {
        var account = GetAccount(accountNumber);
        if (account == null)
        {
            message = "Nomor rekening tidak ditemukan.";
            return false;
        }

        if (amount <= 0)
        {
            message = "Nominal setoran harus lebih besar dari Rp 0.";
            return false;
        }

        if (account.Deposit(amount, "Setor Tunai Mandiri"))
        {
            message = $"Berhasil setor tunai Rp {amount:N0}. Saldo baru: Rp {account.Balance:N0}.";
            return true;
        }

        message = "Gagal memproses setoran.";
        return false;
    }

    /// <summary>
    /// Menarik dana dari rekening nasabah.
    /// </summary>
    public bool Withdraw(string accountNumber, decimal amount, out string message)
    {
        var account = GetAccount(accountNumber);
        if (account == null)
        {
            message = "Nomor rekening tidak ditemukan.";
            return false;
        }

        if (account.Withdraw(amount, "Tarik Tunai Mandiri", out string err))
        {
            message = $"Berhasil tarik tunai Rp {amount:N0}. Saldo tersisa: Rp {account.Balance:N0}.";
            return true;
        }

        message = err;
        return false;
    }

    /// <summary>
    /// Mentransfer dana antar-rekening nasabah.
    /// </summary>
    public bool Transfer(string sourceAccountNumber, string targetAccountNumber, decimal amount, string note, out string message)
    {
        var sourceAccount = GetAccount(sourceAccountNumber);
        if (sourceAccount == null)
        {
            message = "Rekening pengirim tidak ditemukan.";
            return false;
        }

        var targetAccount = GetAccount(targetAccountNumber);
        if (targetAccount == null)
        {
            message = "Nomor rekening penerima tidak ditemukan di sistem.";
            return false;
        }

        if (sourceAccount.TransferTo(targetAccount, amount, note, out string err))
        {
            message = $"Transfer sebesar Rp {amount:N0} ke Rek. {targetAccount.AccountNumber} ({targetAccount.Owner.FullName}) berhasil!";
            return true;
        }

        message = err;
        return false;
    }

    /// <summary>
    /// Mengisi data awal untuk mempermudah demonstrasi dan simulasi.
    /// </summary>
    private void SeedSampleData()
    {
        // Rekening Tabungan Budi (PIN: 123456)
        OpenAccount("Budi Santoso", "081234567890", "123456", AccountType.Savings, 500_000m);

        // Rekening Giro Siti (PIN: 654321)
        OpenAccount("Siti Rahma", "089876543210", "654321", AccountType.Current, 1_500_000m);
    }
}
