using System.Security.Cryptography;
using System.Text;

namespace SistVenta.Api.Helpers;

public static class PasswordHelper
{
    private const string Salt = "SistemaPuntoVenta2025";

    public static string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("La contraseña no puede estar vacía");

        using var sha256 = SHA256.Create();
        var passwordWithSalt = password + Salt;
        var bytes = Encoding.UTF8.GetBytes(passwordWithSalt);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public static bool VerifyPassword(string password, string hash)
    {
        try
        {
            var hashGenerado = HashPassword(password);
            return hashGenerado == hash;
        }
        catch
        {
            return false;
        }
    }
}
