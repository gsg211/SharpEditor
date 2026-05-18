using BackEnd.business.DTOs;
using BackEnd.business.Exceptions;
using BackEnd.business.interfaces;
using BackEnd.business.Security;
using BackEnd.persistence.entities;

namespace BackEnd.business.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}

public class AuthService : IAuthService
{
    private readonly IUnitOfWork     _uow;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtService     _jwt;

    public AuthService(IUnitOfWork uow, IPasswordHasher hasher, IJwtService jwt)
    {
        _uow    = uow;
        _hasher = hasher;
        _jwt    = jwt;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
    {
        req = req with 
        { 
            Email = req.Email.Trim().ToLowerInvariant(),
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
        var existingByEmail    = await _uow.Users.GetByEmailAsync(req.Email);
        if (existingByEmail is not null)
            throw new ConflictException("Email already in use.");

        var existingByUsername = await _uow.Users.GetByUsernameAsync(req.Username);
        if (existingByUsername is not null)
            throw new ConflictException("Username already taken.");

        // ── create user ───────────────────────────────────────────────────────
        var user = new User
        {
            Username     = req.Username,
            Email        = req.Email,
            PasswordHash = _hasher.Hash(req.Password),
            CreatedAt    = DateTime.UtcNow,
        };

        await _uow.Users.AddAsync(user);
        await _uow.CompleteAsync();

        // ACUM: Nu mai generăm token, returnăm doar un răspuns simplu de confirmare
        return new AuthResponse(Token: null, Message: "User registered successfully.");
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest req)
    {
        
        if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
            throw new ValidationException("Email and password are required.");

        var cleanEmail = req.Email.Trim().ToLowerInvariant();

        var user = await _uow.Users.GetByEmailAsync(cleanEmail);

        if (user is null || !_hasher.Verify(req.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid credentials.");

        return new AuthResponse(_jwt.GenerateToken(user.Id, user.Email));
    }
}
