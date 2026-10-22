using SimpleBankingSystem.Models;
using SimpleBankingSystem.Services;

namespace SimpleBankingSystem.UI;

/// <summary>
/// Menu dan antarmuka khusus Nasabah setelah login/akses rekening.
/// </summary>
public class CustomerPortal
{
    private readonly BankService _bankService;
    private readonly Account _account;

    public CustomerPortal(BankService bankService, Account account)
    {
        _bankService = bankService;
        _account = account;
    }

    public void Show()
    {
        bool inSession = true;
        while (inSession)
        {
            Console.Clear();
            ConsoleHelper.PrintHeader($"PORTAL NASABAH - {_account.Owner.FullName}");
            Console.WriteLine($"Nomor Rekening : {_account.AccountNumber}");
            Console.WriteLine($"Tipe Rekening  : {(_account.Type == Enums.AccountType.Savings ? "Tabungan (Savings)" : "Giro (Current)")}");
            Console.WriteLine($"Saldo Saat Ini : Rp {_account.Balance:N0}");
            Console.WriteLine(new string('-', 60));
            Console.WriteLine("1. Cek Detail Akun & Profil");
            Console.WriteLine("2. Setor Tunai (Deposit)");
            Console.WriteLine("3. Tarik Tunai (Withdrawal)");
            Console.WriteLine("4. Transfer Dana Antar-Rekening");
            Console.WriteLine("5. Cetak Mutasi Rekening (Riwayat Transaksi)");
            Console.WriteLine("6. Ubah PIN Keamanan");
            Console.WriteLine("0. Keluar (Logout)");
            Console.WriteLine(new string('-', 60));
            Console.Write("Pilih menu [0-6]: ");

            string? choice = Console.ReadLine()?.Trim();
            switch (choice)
            {
                case "1":
                    ShowAccountDetails();
                    break;
                case "2":
                    HandleDeposit();
                    break;
                case "3":
                    HandleWithdrawal();
                    break;
                case "4":
                    HandleTransfer();
                    break;
                case "5":
                    ShowMutationStatement();
                    break;
                case "6":
                    HandleChangePin();
                    break;
                case "0":
                    inSession = false;
                    ConsoleHelper.Info("Anda telah berhasil logout dari sesi nasabah.");
                    ConsoleHelper.Pause();
                    break;
                default:
                    ConsoleHelper.Error("Pilihan tidak valid!");
                    ConsoleHelper.Pause();
                    break;
            }
        }
    }

    private void ShowAccountDetails()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("INFORMASI REKENING & PROFIL NASABAH");
        Console.WriteLine($"ID Nasabah       : {_account.Owner.Id}");
        Console.WriteLine($"Nama Pemilik     : {_account.Owner.FullName}");
        Console.WriteLine($"Nomor Telepon    : {_account.Owner.PhoneNumber}");
        Console.WriteLine($"Nomor Rekening   : {_account.AccountNumber}");
        Console.WriteLine($"Jenis Rekening   : {(_account.Type == Enums.AccountType.Savings ? "Tabungan (Savings)" : "Giro (Current)")}");
        Console.WriteLine($"Tanggal Dibuat   : {_account.CreatedAt:dd MMMM yyyy HH:mm:ss}");
        Console.WriteLine($"Saldo Terkini    : Rp {_account.Balance:N0}");

        if (_account is SavingsAccount savings)
        {
            Console.WriteLine($"Saldo Minimum    : Rp {savings.MinimumBalance:N0} (Wajib mengendap)");
        }
        else if (_account is CurrentAccount current)
        {
            Console.WriteLine($"Limit Overdraft  : Rp {current.OverdraftLimit:N0} (Batas cerukan)");
        }

        Console.WriteLine($"Total Transaksi  : {_account.Transactions.Count} kali");
        ConsoleHelper.Pause();
    }

    private void HandleDeposit()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("TRANSAKSI: SETOR TUNAI");
        Console.WriteLine($"Rekening Tujuan : {_account.AccountNumber} ({_account.Owner.FullName})");
        Console.WriteLine($"Saldo Saat Ini  : Rp {_account.Balance:N0}\n");

        decimal amount = ConsoleHelper.ReadDecimal("Masukkan nominal setoran (Rp): ", min: 10_000);

        if (_bankService.Deposit(_account.AccountNumber, amount, out string msg))
        {
            ConsoleHelper.Success(msg);
        }
        else
        {
            ConsoleHelper.Error(msg);
        }
        ConsoleHelper.Pause();
    }

    private void HandleWithdrawal()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("TRANSAKSI: TARIK TUNAI");
        Console.WriteLine($"Rekening Sumber : {_account.AccountNumber} ({_account.Owner.FullName})");
        Console.WriteLine($"Saldo Saat Ini  : Rp {_account.Balance:N0}\n");

        decimal amount = ConsoleHelper.ReadDecimal("Masukkan nominal penarikan (Rp): ", min: 10_000);

        if (_bankService.Withdraw(_account.AccountNumber, amount, out string msg))
        {
            ConsoleHelper.Success(msg);
        }
        else
        {
            ConsoleHelper.Error(msg);
        }
        ConsoleHelper.Pause();
    }

    private void HandleTransfer()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("TRANSAKSI: TRANSFER DANA");
        Console.WriteLine($"Rekening Sumber : {_account.AccountNumber} ({_account.Owner.FullName})");
        Console.WriteLine($"Saldo Saat Ini  : Rp {_account.Balance:N0}\n");

        string targetAcc = ConsoleHelper.ReadNonEmpty("Masukkan nomor rekening tujuan (contoh: SAV-1001, CUR-2001): ");

        var target = _bankService.GetAccount(targetAcc);
        if (target == null)
        {
            ConsoleHelper.Error("Rekening tujuan tidak ditemukan di sistem!");
            ConsoleHelper.Pause();
            return;
        }

        if (target.AccountNumber.Equals(_account.AccountNumber, StringComparison.OrdinalIgnoreCase))
        {
            ConsoleHelper.Error("Tidak dapat mentransfer ke rekening sendiri!");
            ConsoleHelper.Pause();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Rekening Penerima Terdeteksi: {target.AccountNumber} a.n {target.Owner.FullName}");
        Console.ResetColor();

        decimal amount = ConsoleHelper.ReadDecimal("Masukkan nominal transfer (Rp): ", min: 10_000);
        string note = ConsoleHelper.ReadNonEmpty("Catatan/Keterangan transfer: ");

        // Konfirmasi transfer
        Console.WriteLine("\n--- Konfirmasi Transfer ---");
        Console.WriteLine($"Tujuan : {target.AccountNumber} ({target.Owner.FullName})");
        Console.WriteLine($"Jumlah : Rp {amount:N0}");
        Console.WriteLine($"Catatan: {note}");
        Console.Write("Lanjutkan transfer? (y/n): ");
        string? confirm = Console.ReadLine()?.Trim().ToLowerInvariant();

        if (confirm == "y" || confirm == "ya")
        {
            if (_bankService.Transfer(_account.AccountNumber, target.AccountNumber, amount, note, out string msg))
            {
                ConsoleHelper.Success(msg);
            }
            else
            {
                ConsoleHelper.Error(msg);
            }
        }
        else
        {
            ConsoleHelper.Info("Transfer dibatalkan.");
        }

        ConsoleHelper.Pause();
    }

    private void ShowMutationStatement()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader($"MUTASI REKENING - {_account.AccountNumber}");
        Console.WriteLine($"Nama Nasabah : {_account.Owner.FullName}");
        Console.WriteLine($"Saldo Terkini: Rp {_account.Balance:N0}");
        Console.WriteLine(new string('=', 95));
        Console.WriteLine(string.Format("{0,-12} | {1,-19} | {2,-12} | {3,15} | {4,15} | {5}",
            "ID TXN", "TANGGAL & WAKTU", "TIPE", "NOMINAL (Rp)", "SALDO (Rp)", "KETERANGAN"));
        Console.WriteLine(new string('-', 95));

        if (_account.Transactions.Count == 0)
        {
            Console.WriteLine("Belum ada riwayat transaksi.");
        }
        else
        {
            foreach (var txn in _account.Transactions)
            {
                string sign = (txn.Type == Enums.TransactionType.Deposit || txn.Type == Enums.TransactionType.TransferIn) ? "+" : "-";
                Console.ForegroundColor = sign == "+" ? ConsoleColor.Green : ConsoleColor.Yellow;

                Console.WriteLine(string.Format("{0,-12} | {1,-19:yyyy-MM-dd HH:mm} | {2,-12} | {3,15} | {4,15:N0} | {5}",
                    txn.Id,
                    txn.Timestamp,
                    txn.Type,
                    $"{sign} Rp {txn.Amount:N0}",
                    txn.BalanceAfter,
                    txn.Description));

                Console.ResetColor();
            }
        }

        Console.WriteLine(new string('=', 95));
        ConsoleHelper.Pause();
    }

    private void HandleChangePin()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("UBAH PIN KEAMANAN NASABAH");

        string oldPin = ConsoleHelper.ReadPin("Masukkan PIN Lama: ");
        if (!_account.Owner.VerifyPin(oldPin))
        {
            ConsoleHelper.Error("PIN Lama yang Anda masukkan salah!");
            ConsoleHelper.Pause();
            return;
        }

        string newPin = ConsoleHelper.ReadPin("Masukkan PIN Baru (minimal 4 digit angka): ");
        if (newPin.Length < 4)
        {
            ConsoleHelper.Error("PIN Baru harus memiliki minimal 4 digit angka!");
            ConsoleHelper.Pause();
            return;
        }

        string confirmPin = ConsoleHelper.ReadPin("Konfirmasi PIN Baru: ");
        if (newPin != confirmPin)
        {
            ConsoleHelper.Error("Konfirmasi PIN Baru tidak cocok!");
            ConsoleHelper.Pause();
            return;
        }

        if (_account.Owner.ChangePin(oldPin, newPin))
        {
            ConsoleHelper.Success("PIN berhasil diperbarui! Gunakan PIN baru untuk transaksi berikutnya.");
        }
        else
        {
            ConsoleHelper.Error("Gagal memperbarui PIN.");
        }

        ConsoleHelper.Pause();
    }
}
