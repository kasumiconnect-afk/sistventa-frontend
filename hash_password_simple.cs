using System;
using System.Security.Cryptography;
using System.Text;

Console.WriteLine("Hash calculador para SistVenta");
Console.WriteLine("Salt: SistemaPuntoVenta2025");
Console.WriteLine();

string[] passwords = { "123456", "admin", "admin123", "password", "test123" };
const string Salt = "SistemaPuntoVenta2025";

using var sha256 = SHA256.Create();
foreach (var password in passwords)
{
    var passwordWithSalt = password + Salt;
    var bytes = Encoding.UTF8.GetBytes(passwordWithSalt);
    var hash = sha256.ComputeHash(bytes);
    string hashString = Convert.ToBase64String(hash);
    Console.WriteLine($"Password: {password}");
    Console.WriteLine($"Hash: {hashString}");
    Console.WriteLine();
}
