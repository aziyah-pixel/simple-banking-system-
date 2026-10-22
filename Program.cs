using SimpleBankingSystem.Enums;
using SimpleBankingSystem.Models;
using SimpleBankingSystem.Services;
using SimpleBankingSystem.UI;

namespace SimpleBankingSystem;

internal class Program
{
    private static readonly BankService _bankService = new();

    static void Main(string[] args)
    {
        Console.Title = "Sistem Simulasi Perbankan - C# .NET";
        bool isRunning = true;

        while (isRunning)
        {
            Console.Clear();
            ConsoleHelper.PrintHeader("BANK SIMULASI DIGITAL - MENU UTAMA");
            Console.WriteLine("Selamat datang di Sistem Layanan Perbankan Digital!");
            Console.WriteLine(new string('-', 60));
            Console.WriteLine("1. Buka Rekening Baru (Pendaftaran Nasabah)");
            Console.WriteLine("2. Masuk ke Akun Nasabah (Layanan Nasabah)");
            Console.WriteLine("3. Lihat Daftar Akun Demo / Tersedia (Untuk Pengujian)");
            Console.WriteLine("0. Keluar Aplikasi");
            Console.WriteLine(new string('-', 60));
            Console.Write("Pilih menu [0-3]: ");

            string? choice = Console.ReadLine()?.Trim();
            switch (choice)
            {
                case "1":
                    OpenNewAccount();
                    break;
                case "2":
                    LoginCustomer();
                    break;
                case "3":
                    ShowRegisteredAccounts();
                    break;
                case "0":
                    isRunning = false;
                    Console.Clear();
                    ConsoleHelper.PrintHeader("TERIMA KASIH");
                    Console.WriteLine("Terima kasih telah menggunakan Sistem Perbankan Digital kami.");
                    break;
                default:
                    ConsoleHelper.Error("Pilihan tidak valid!");
                    ConsoleHelper.Pause();
                    break;
            }
        }
    }

    private static void OpenNewAccount()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("PEMBUKAAN REKENING NASABAH BARU");

        string fullName = ConsoleHelper.ReadNonEmpty("Nama Lengkap Nasabah   : ");
        string phoneNumber = ConsoleHelper.ReadNonEmpty("Nomor Telepon/HP       : ");
        
        string pin;
        while (true)
        {
            pin = ConsoleHelper.ReadPin("Buat PIN Keamanan (6 angka): ");
            if (pin.Length >= 4)
            {
                break;
            }
            ConsoleHelper.Error("PIN harus terdiri dari minimal 4 digit angka!");
        }

        Console.WriteLine("\nPilih Jenis Rekening:");
        Console.WriteLine("  1. Rekening Tabungan (Savings) - Saldo mengendap minimum Rp 50.000");
        Console.WriteLine("  2. Rekening Giro (Current) - Mendukung overdraft limit hingga Rp 1.000.000");
        
        AccountType type = AccountType.Savings;
        while (true)
        {
            Console.Write("Pilih jenis rekening [1/2]: ");
            string? typeChoice = Console.ReadLine()?.Trim();
            if (typeChoice == "1")
            {
                type = AccountType.Savings;
                break;
            }
            else if (typeChoice == "2")
            {
                type = AccountType.Current;
                break;
            }
            ConsoleHelper.Error("Pilihan tidak valid. Ketik 1 untuk Tabungan atau 2 untuk Giro.");
        }

        decimal minDeposit = type == AccountType.Savings ? 50_000m : 100_000m;
        decimal initialDeposit = ConsoleHelper.ReadDecimal($"Setoran Awal (Minimal Rp {minDeposit:N0}): ", min: minDeposit);

        Account newAccount = _bankService.OpenAccount(fullName, phoneNumber, pin, type, initialDeposit);

        ConsoleHelper.Success("Rekening Nasabah Berhasil Dibuat!");
        Console.WriteLine(new string('-', 60));
        Console.WriteLine($"Nomor Rekening : {newAccount.AccountNumber}");
        Console.WriteLine($"Nama Nasabah   : {newAccount.Owner.FullName}");
        Console.WriteLine($"Jenis Rekening : {(newAccount.Type == AccountType.Savings ? "Tabungan (Savings)" : "Giro (Current)")}");
        Console.WriteLine($"Saldo Awal     : Rp {newAccount.Balance:N0}");
        Console.WriteLine(new string('-', 60));
        ConsoleHelper.Info("Harap catat Nomor Rekening dan jaga kerahasiaan PIN Anda.");

        ConsoleHelper.Pause();
    }

    private static void LoginCustomer()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("MASUK KE PORTAL NASABAH");

        string accNumber = ConsoleHelper.ReadNonEmpty("Masukkan Nomor Rekening (contoh: SAV-1001): ");
        string pin = ConsoleHelper.ReadPin("Masukkan PIN Keamanan: ");

        if (_bankService.Authenticate(accNumber, pin, out Account? account, out string errorMessage))
        {
            ConsoleHelper.Success($"Selamat datang kembali, {account!.Owner.FullName}!");
            Thread.Sleep(800);
            
            CustomerPortal portal = new CustomerPortal(_bankService, account);
            portal.Show();
        }
        else
        {
            ConsoleHelper.Error(errorMessage);
            ConsoleHelper.Pause();
        }
    }

    private static void ShowRegisteredAccounts()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("DAFTAR REKENING TERDAFTAR (DATA SIMULASI)");
        Console.WriteLine("Gunakan data berikut untuk mencoba login dan transfer dana:\n");

        var accounts = _bankService.GetAllAccounts();
        Console.WriteLine(string.Format("{0,-10} | {1,-20} | {2,-12} | {3,15}",
            "NO. REK", "NAMA NASABAH", "TIPE", "SALDO (Rp)"));
        Console.WriteLine(new string('-', 65));

        foreach (var acc in accounts)
        {
            Console.WriteLine(string.Format("{0,-10} | {1,-20} | {2,-12} | {3,15:N0}",
                acc.AccountNumber,
                acc.Owner.FullName,
                acc.Type,
                acc.Balance));
        }

        Console.WriteLine(new string('-', 65));
        ConsoleHelper.Info("Catatan: Akun Budi (PIN: 123456), Akun Siti (PIN: 654321).");
        ConsoleHelper.Pause();
    }
}
