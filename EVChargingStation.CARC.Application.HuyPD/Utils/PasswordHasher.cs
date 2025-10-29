using System.Security.Cryptography;
using System.Text;

namespace EVChargingStation.CARC.Application.HuyPD.Utils;

public class PasswordHasher
{
    private const int WorkFactor = 10;

    // Hash a plaintext password using BCrypt
    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty.");

        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);
    }
    // Verify a plaintext password against a stored hash
    public bool VerifyPassword(string password, string storedHash, out string? newHash)
    {
        newHash = null;

        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
            return false;

        bool isValid = BCrypt.Net.BCrypt.Verify(password, storedHash);

        // If password is correct but the hash uses an outdated cost factor, rehash
        if (isValid && BCrypt.Net.BCrypt.PasswordNeedsRehash(storedHash, WorkFactor))
        {
            newHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);
        }

        return isValid;
    }
}