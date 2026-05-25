// =============================================================================
// File:        JwtService.cs
// Author:      Gorea Sabin-Gabriel
// Description: Defines the IJwtService interface and its implementation.
//              JwtService handles JWT token generation and validation using
//              HMAC-SHA256 signing. Configuration values (secret, issuer,
//              audience, expiry) are loaded from the application settings.
// =============================================================================

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Core.business.Security;

// Contract for JWT generation and validation
public interface IJwtService
{
    string GenerateToken(int userId, string email);
    int? ValidateToken(string token);
}

public class JwtService : IJwtService
{
    // JWT configuration values loaded from application settings
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiryMinutes;

    // Read JWT settings from configuration; apply sensible defaults where optional
    public JwtService(IConfiguration config)
    {
        _secret        = config["Jwt:Secret"]   ?? throw new InvalidOperationException("Jwt:Secret not configured");
        _issuer        = config["Jwt:Issuer"]   ?? "BackEnd";
        _audience      = config["Jwt:Audience"] ?? "BackEnd";
        _expiryMinutes = int.TryParse((string?)config["Jwt:ExpiryMinutes"], out var m) ? m : 60;
    }

    // Builds and signs a JWT containing the user ID, email, and a unique token ID
    public string GenerateToken(int userId, string email)
    {
        // Create the signing key from the configured secret
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Define the claims embedded in the token
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),  // User ID as subject
            new Claim(JwtRegisteredClaimNames.Email, email),            // User email
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique token ID
        };

        // Build the token with issuer, audience, claims, expiry, and signing credentials
        var token = new JwtSecurityToken(
            issuer:             _issuer,
            audience:           _audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddMinutes(_expiryMinutes),
            signingCredentials: creds);

        // Serialize the token to a compact string representation
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Validates a JWT string and returns the user ID extracted from the subject claim.
    // Returns null if the token is invalid, expired, or cannot be parsed.
    public int? ValidateToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));

            // Validate signature, issuer, audience, and token lifetime
            handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey         = key,
                ValidateIssuer           = true,
                ValidIssuer              = _issuer,
                ValidateAudience         = true,
                ValidAudience            = _audience,
                ValidateLifetime         = true,
                ClockSkew                = TimeSpan.Zero, // No tolerance for expiry
            }, out var validated);

            // Extract and return the user ID from the subject claim
            var jwt = (JwtSecurityToken)validated;
            return int.Parse((string)jwt.Subject);
        }
        catch
        {
            // Any validation failure returns null instead of throwing
            return null;
        }
    }
}