// =============================================================================
// File:        PasswordHasher.cs
// Author:      Gorea Sabin-Gabriel
// Description: Defines the IPasswordHasher interface and its implementation.
//              PasswordHasher securely hashes passwords using PBKDF2 with
//              HMAC-SHA256, a random salt, and 100 000 iterations. Stored
//              hashes use the format "base64(salt):base64(hash)". Verification
//              uses a constant-time comparison to prevent timing attacks.
// =============================================================================

using System.Security.Cryptography;
using System.Text;

namespace Core.business.Security;

// Contract for hashing passwords and verifying them against stored hashes
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize   = 16;       // Salt length in bytes
    private const int HashSize   = 32;       // Derived key length in bytes (256 bits)
    private const int Iterations = 100_000;  // PBKDF2 iteration count

    // Hashes a plain-text password using PBKDF2-SHA256 with a random salt.
    // Returns the result as "base64(salt):base64(hash)".
    public string Hash(string password)
    {
        // Generate a cryptographically secure random salt
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        // Derive the hash using PBKDF2 with SHA-256
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        // format:  base64(salt):base64(hash)
        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    // Verifies a plain-text password against a previously stored hash string.
    // Returns true if the password matches, false otherwise.
    public bool Verify(string password, string storedHash)
    {
        // Split the stored value into salt and hash components
        var parts = storedHash.Split(':');
        if (parts.Length != 2) return false;

        var salt         = Convert.FromBase64String(parts[0]);
        var expectedHash = Convert.FromBase64String(parts[1]);

        // Re-derive the hash from the provided password using the stored salt
        var actualHash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        // Use constant-time comparison to prevent timing-based attacks
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}