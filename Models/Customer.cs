namespace SimpleBankingSystem.Models;

/// <summary>
/// Merepresentasikan entitas Nasabah bank.
/// </summary>
public class Customer
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    private string Pin { get; set; }

    public Customer(string id, string fullName, string phoneNumber, string pin)
    {
        Id = id;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Pin = pin;
    }

    /// <summary>
    /// Memverifikasi PIN nasabah (Enkapsulasi keamanan PIN).
    /// </summary>
    public bool VerifyPin(string inputPin)
    {
        return Pin == inputPin;
    }

    /// <summary>
    /// Memperbarui PIN nasabah dengan verifikasi PIN lama.
    /// </summary>
    public bool ChangePin(string oldPin, string newPin)
    {
        if (VerifyPin(oldPin))
        {
            Pin = newPin;
            return true;
        }
        return false;
    }
}
