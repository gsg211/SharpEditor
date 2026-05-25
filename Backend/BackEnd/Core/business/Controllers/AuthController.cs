// =============================================================================
// File:        AuthController.cs
// Author:      Gorea Sabin Gabriel
// Description: REST API controller that exposes authentication endpoints
//              (registration and login). Delegates business logic to
//              IAuthService and maps service-layer exceptions to the
//              appropriate HTTP status codes.
// =============================================================================

using Core.business.DTOs;
using Core.business.Exceptions;
using Core.business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.business.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    // Service responsible for authentication logic (register / login)
    private readonly IAuthService _auth;

    // Inject IAuthService via constructor
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
            // Delegate registration to the auth service
            var result = await _auth.RegisterAsync(req);

            // Return 201 Created with the generated auth response (e.g. JWT)
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (ValidationException ex)  { return BadRequest(new { error = ex.Message }); }  // 400 – invalid input
        catch (ConflictException ex)    { return Conflict(new { error = ex.Message }); }     // 409 – user already exists
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
            // Delegate credential verification to the auth service
            var result = await _auth.LoginAsync(req);

            // Return 200 OK with the auth response (e.g. JWT token)
            return Ok(result);
        }
        catch (ValidationException ex)    { return BadRequest(new { error = ex.Message }); }   // 400 – invalid input
        catch (UnauthorizedException ex)  { return Unauthorized(new { error = ex.Message }); } // 401 – wrong credentials
    }
}