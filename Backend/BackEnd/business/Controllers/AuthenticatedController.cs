using System.Security.Claims;
using BackEnd.business.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.business.Controllers;

/// <summary>
/// Provides a helper that extracts the authenticated user id from the JWT
/// claims injected by the authentication middleware.
/// </summary>
public abstract class AuthenticatedController : ControllerBase
{
    protected int CurrentUserId
    {
        get
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? User.FindFirstValue("sub");

            if (sub is null || !int.TryParse(sub, out var id))
                throw new UnauthorizedException("Invalid or missing token.");

            return id;
        }
    }
}
