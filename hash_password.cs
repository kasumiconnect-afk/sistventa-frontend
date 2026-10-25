using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        const string Salt = "SistemaPuntoVenta2025";
        string password = "admin123";

        using var sha256 = SHA256.Create();
        var passwordWithSalt = password + Salt;
        var bytes = Encoding.UTF8.GetBytes(passwordWithSalt);
        var hash = sha256.ComputeHash(bytes);
        string hashString = Convert.ToBase64String(hash);

        Console.WriteLine($"Password: {password}");
        Console.WriteLine($"Hash: {hashString}");
    }
}
