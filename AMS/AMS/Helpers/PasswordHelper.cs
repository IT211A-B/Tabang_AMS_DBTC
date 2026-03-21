using System.Security.Cryptography;
using System.Text;

namespace AMS.Helpers
{
    // Utility class for hashing and verifying passwords
    // Uses BCrypt-style salted SHA-256 for simplicity;
    // in production consider using BCrypt.Net or ASP.NET Core Identity.
    public static class PasswordHelper
    {
        // Hash a plain-text password using SHA-256 with a random salt.
        // Format stored: "salt:hash"
        public static string HashPassword(string plainText)
        {
            // Generate a random 16-byte salt to prevent rainbow-table attacks
            var salt = RandomNumberGenerator.GetBytes(16);
            var saltBase64 = Convert.ToBase64String(salt);

            // Combine salt + password then hash
            var combined = Encoding.UTF8.GetBytes(saltBase64 + plainText);
            var hash = SHA256.HashData(combined);
            var hashBase64 = Convert.ToBase64String(hash);

            // Store both salt and hash so we can verify later
            return $"{saltBase64}:{hashBase64}";
        }

        // Verify a plain-text password against a stored hash string
        public static bool VerifyPassword(string plainText, string storedHash)
        {
            // Split stored value back into salt and hash parts
            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;

            var saltBase64 = parts[0];
            var expectedHashBase64 = parts[1];

            // Re-hash the provided plain text with the stored salt
            var combined = Encoding.UTF8.GetBytes(saltBase64 + plainText);
            var hash = SHA256.HashData(combined);
            var actualHashBase64 = Convert.ToBase64String(hash);

            // Compare in constant time to avoid timing attacks
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(actualHashBase64),
                Encoding.UTF8.GetBytes(expectedHashBase64));
        }
    }
}