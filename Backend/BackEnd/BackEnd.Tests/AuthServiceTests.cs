using Core.business.DTOs;
using Core.business.Exceptions;
using Core.business.interfaces;
using Core.business.Security;
using Core.business.Services;
using Core.persistence.entities;
using Moq;

namespace BackEnd.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork>     _uow     = new();
    private readonly Mock<IUserRepository> _users   = new();
    private readonly Mock<IPasswordHasher> _hasher  = new();
    private readonly Mock<IJwtService>     _jwt     = new();
    private readonly IAuthService          _sut;

    public AuthServiceTests()
    {
        _uow.Setup(u => u.Users).Returns(_users.Object);
        _sut = new AuthService(_uow.Object, _hasher.Object, _jwt.Object);
    }

    // ── Register ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Register_ValidRequest_ReturnsSuccessMessage()
    {
        _users.Setup(u => u.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        _users.Setup(u => u.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        _hasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed");

        var result = await _sut.RegisterAsync(new RegisterRequest("alice", "alice@example.com", "password123"));

        Assert.Null(result.Token);
        Assert.Contains("registered", result.Message, StringComparison.OrdinalIgnoreCase);
        _uow.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Theory]
    [InlineData("", "alice@example.com", "password123")]   // empty username
    [InlineData("alice", "not-an-email", "password123")]   // invalid email
    [InlineData("alice", "alice@example.com", "123")]      // short password
    public async Task Register_InvalidInput_ThrowsValidationException(
        string username, string email, string password)
    {
        await Assert.ThrowsAsync<ValidationException>(
            () => _sut.RegisterAsync(new RegisterRequest(username, email, password)));
    }

    [Fact]
    public async Task Register_DuplicateEmail_ThrowsConflictException()
    {
        _users.Setup(u => u.GetByEmailAsync("alice@example.com"))
              .ReturnsAsync(new User { Email = "alice@example.com" });

        await Assert.ThrowsAsync<ConflictException>(
            () => _sut.RegisterAsync(new RegisterRequest("alice", "alice@example.com", "password123")));
    }

    [Fact]
    public async Task Register_DuplicateUsername_ThrowsConflictException()
    {
        _users.Setup(u => u.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        _users.Setup(u => u.GetByUsernameAsync("alice"))
              .ReturnsAsync(new User { Username = "alice" });

        await Assert.ThrowsAsync<ConflictException>(
            () => _sut.RegisterAsync(new RegisterRequest("alice", "new@example.com", "password123")));
    }

    [Fact]
    public async Task Register_NormalizesEmailToLowercase()
    {
        _users.Setup(u => u.GetByEmailAsync("alice@example.com")).ReturnsAsync((User?)null);
        _users.Setup(u => u.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        _hasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed");

        await _sut.RegisterAsync(new RegisterRequest("alice", "ALICE@EXAMPLE.COM", "password123"));

        _users.Verify(u => u.GetByEmailAsync("alice@example.com"), Times.Once);
    }

    // ── Login ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        var user = new User { Id = 1, Email = "alice@example.com", PasswordHash = "hashed" };
        _users.Setup(u => u.GetByEmailAsync("alice@example.com")).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("password123", "hashed")).Returns(true);
        _jwt.Setup(j => j.GenerateToken(1, "alice@example.com")).Returns("jwt-token");

        var result = await _sut.LoginAsync(new LoginRequest("alice@example.com", "password123"));

        Assert.Equal("jwt-token", result.Token);
    }

    [Fact]
    public async Task Login_UnknownEmail_ThrowsUnauthorizedException()
    {
        _users.Setup(u => u.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<UnauthorizedException>(
            () => _sut.LoginAsync(new LoginRequest("x@x.com", "password123")));
    }

    [Fact]
    public async Task Login_WrongPassword_ThrowsUnauthorizedException()
    {
        var user = new User { Id = 1, Email = "alice@example.com", PasswordHash = "hashed" };
        _users.Setup(u => u.GetByEmailAsync("alice@example.com")).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("wrong", "hashed")).Returns(false);

        await Assert.ThrowsAsync<UnauthorizedException>(
            () => _sut.LoginAsync(new LoginRequest("alice@example.com", "wrong")));
    }

    [Theory]
    [InlineData("", "password123")]
    [InlineData("alice@example.com", "")]
    public async Task Login_MissingFields_ThrowsValidationException(string email, string password)
    {
        await Assert.ThrowsAsync<ValidationException>(
            () => _sut.LoginAsync(new LoginRequest(email, password)));
    }
}
