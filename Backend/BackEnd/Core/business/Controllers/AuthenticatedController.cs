// =============================================================================
// File:        AuthenticatedController.cs
// Author:      Gorea Sabin-Gabriel
// Description: Abstract base controller that provides a convenience property
//              for retrieving the currently authenticated user's ID from the
//              JWT claims injected by ASP.NET Core's authentication middleware.
//              All controllers that require an authenticated user should
//              inherit from this class.
// =============================================================================

using System.Security.Claims;
using Core.business.Exceptions;
using Core.persistence.entities;
using Microsoft.AspNetCore.Mvc;

namespace Core.business.Controllers;

/// <summary>
/// Provides a helper that extracts the authenticated user id from the JWT
/// claims injected by the authentication middleware.
/// </summary>
public abstract class AuthenticatedController : ControllerBase
{
    // Reads the current user's ID from the JWT claims.
    // Throws UnauthorizedException if the claim is absent or cannot be parsed.
    protected int CurrentUserId
    {
        get
        {
            // Try the standard NameIdentifier claim first, then fall back to "sub"
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub");

            // Reject the request if the claim is missing or not a valid integer
            if (sub is null || !int.TryParse((string?)sub, out var id))
                throw new UnauthorizedException("Invalid or missing token.");

            return id;
        }
    }
}