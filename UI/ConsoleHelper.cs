namespace SimpleBankingSystem.UI;

/// <summary>
/// Pembantu tampilan konsol untuk mempercantik UI dan validasi input.
/// </summary>
public static class ConsoleHelper
{
    public static void PrintHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string('=', 60));
        Console.WriteLine($"  {title.ToUpperInvariant()}");
        Console.WriteLine(new string('=', 60));
        Console.ResetColor();
    }

    public static void Success(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n[✓ SUKSES] {message}");
        Console.ResetColor();
    }

    public static void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[✗ GAGAL] {message}");
        Console.ResetColor();
    }

    public static void Info(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[i] {message}");
        Console.ResetColor();
    }

    public static void Pause()
    {
        Console.WriteLine("\nTekan sembarang tombol untuk melanjutkan...");
        Console.ReadKey(true);
    }

    public static string ReadNonEmpty(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(input))
            {
                return input;
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Input tidak boleh kosong!");
            Console.ResetColor();
        }
    }

    public static decimal ReadDecimal(string prompt, decimal min = 0)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim().Replace(".", "").Replace(",", "");
            if (decimal.TryParse(input, out decimal amount) && amount >= min)
            {
                return amount;
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Masukkan angka yang valid (minimal Rp {min:N0})!");
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Membaca input PIN dalam bentuk bintang (*) demi privasi nasabah.
    /// </summary>
    public static string ReadPin(string prompt)
    {
        Console.Write(prompt);
        string pin = string.Empty;
        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }
            if (key.Key == ConsoleKey.Backspace && pin.Length > 0)
            {
                pin = pin[0..^1];
                Console.Write("\b \b");
            }
            else if (char.IsDigit(key.KeyChar))
            {
                pin += key.KeyChar;
                Console.Write("*");
            }
        }
        return pin;
    }
}
