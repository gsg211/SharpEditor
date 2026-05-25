// =============================================================================
// File:        AuthService.cs
// Author:      Gorea Sabin Gabriel
// Description: Defines the IAuthService interface and its implementation.
//              AuthService handles user registration and login logic:
//              validates input, checks uniqueness constraints, hashes
//              passwords, persists new users, and issues JWT tokens on
//              successful login.
// =============================================================================

using Core.business.DTOs;
using Core.business.Exceptions;
using Core.business.interfaces;
using Core.business.Security;
using Core.persistence.entities;

namespace Core.business.Services;

// Contract for authentication operations
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}

public class AuthService : IAuthService
{
    // Dependencies injected via constructor
    private readonly IUnitOfWork     _uow;     // Data access and transaction management
    private readonly IPasswordHasher _hasher;  // Password hashing and verification
    private readonly IJwtService     _jwt;     // JWT token generation

    public AuthService(IUnitOfWork uow, IPasswordHasher hasher, IJwtService jwt)
    {
        _uow    = uow;
        _hasher = hasher;
        _jwt    = jwt;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
    {
        // Normalise email to lowercase and trim whitespace from both fields
        req = req with 
        { 
            Email    = req.Email.Trim().ToLowerInvariant(),
            Username = req.Username.Trim() 
        };
        
        // ── validation ────────────────────────────────────────────────────────
        if (string.IsNullOrWhiteSpace(req.Username))
            throw new ValidationException("Username is required.");

        if (string.IsNullOrWhiteSpace(req.Email) || !req.Email.Contains('@'))
            throw new ValidationException("A valid email is required.");

        if (string.IsNullOrWhiteSpace(req.Password) || req.Password.Length < 6)
            throw new ValidationException("Password must be at least 6 characters.");

        // ── uniqueness check ──────────────────────────────────────────────────
        // Ensure no existing account uses the same email
        var existingByEmail = await _uow.Users.GetByEmailAsync(req.Email);
        if (existingByEmail is not null)
            throw new ConflictException("Email already in use.");

        // Ensure no existing account uses the same username
        var existingByUsername = await _uow.Users.GetByUsernameAsync(req.Username);
        if (existingByUsername is not null)
            throw new ConflictException("Username already taken.");

        // ── create user ───────────────────────────────────────────────────────
        var user = new User
        {
            Username     = req.Username,
            Email        = req.Email,
            PasswordHash = _hasher.Hash(req.Password), // Store only the hashed password
            CreatedAt    = DateTime.UtcNow,
        };

        await _uow.Users.AddAsync(user);
        await _uow.CompleteAsync(); // Persist changes to the database

        // Registration does not issue a token; return a confirmation message only
        return new AuthResponse(Token: null, Message: "User registered successfully.");
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest req)
    {
        // Ensure both fields are provided before querying the database
        if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
            throw new ValidationException("Email and password are required.");

        // Normalise email to match the format used during registration
        var cleanEmail = req.Email.Trim().ToLowerInvariant();

        var user = await _uow.Users.GetByEmailAsync(cleanEmail);

        // Reject if user not found or if the password does not match the stored hash
        if (user is null || !_hasher.Verify(req.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid credentials.");

        // Issue and return a JWT token for the authenticated user
        return new AuthResponse(_jwt.GenerateToken(user.Id, user.Email));
    }
}