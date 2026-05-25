// =============================================================================
// File:        Exceptions.cs
// Author:      Gorea Sabin-Gabriel
// Description: Defines the custom domain exception types used throughout the
//              application. Each exception maps to a specific HTTP error
//              scenario and is caught by the controllers to return the
//              appropriate HTTP status code to the client.
// =============================================================================

namespace Core.business.Exceptions;

// Thrown when a requested resource does not exist (HTTP 404)
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

// Thrown when the authenticated user lacks permission to access a resource (HTTP 403)
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}

// Thrown when an operation conflicts with the current state of a resource (HTTP 409)
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}

// Thrown when authentication credentials are missing or invalid (HTTP 401)
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}

// Thrown when request input fails business-rule or format validation (HTTP 400)
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}