using BackEnd.business.DTOs;
using BackEnd.business.Exceptions;
using BackEnd.business.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.business.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    /// <summary>Register a new user.</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        try
        {
            var result = await _auth.RegisterAsync(req);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (ValidationException ex)  { return BadRequest(new { error = ex.Message }); }
        catch (ConflictException ex)    { return Conflict(new { error = ex.Message }); }
    }

    /// <summary>Authenticate and receive a JWT.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        try
        {
            var result = await _auth.LoginAsync(req);
            return Ok(result);
        }
        catch (ValidationException ex)    { return BadRequest(new { error = ex.Message }); }
        catch (UnauthorizedException ex)  { return Unauthorized(new { error = ex.Message }); }
    }
}
