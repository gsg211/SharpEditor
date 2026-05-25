// =============================================================================
// File:        DTOs.cs
// Author:      Gorea Sabin-Gabriel
// Description: Defines all Data Transfer Objects (DTOs) and request/response
//              records used across the application. Grouped into three
//              sections: authentication, document/shared-document operations,
//              and sharing permissions management.
// =============================================================================

namespace Core.business.DTOs;

// ── Auth ──────────────────────────────────────────────────────────────────────

// Request payload for registering a new user account
public record RegisterRequest(string Username, string Email, string Password);

// Request payload for authenticating an existing user
public record LoginRequest(string Email, string Password);

// Response returned after a successful register or login (contains JWT token)
public record AuthResponse(string? Token, string? Message = null);

// ── Document / SharedDoc ──────────────────────────────────────────────────────

// Request payload for creating a new shared document
public record CreateDocumentRequest(string Title, string? Content);

// Request payload for updating an existing document (includes version for optimistic concurrency)
public record UpdateDocumentRequest(string Content, int Version);

// Full document details returned to the client
public record DocumentDto(int Id, string Title, string Content, int Version,
    DateTime CreatedAt, DateTime UpdatedAt);

// Lightweight summary of a shared document shown in list views
public record SharedDocSummaryDto(int ShareId, string Title, string Permission);

// Full details of a shared document including the document payload and caller's permission
public record SharedDocDetailDto(int ShareId, string Permission, DocumentDto Document);

// ── Sharing ───────────────────────────────────────────────────────────────────

// Request payload for granting a user access to a document
public record CreateShareRequest(int UserId, string Permission);

// Request payload for changing an existing share's permission level
public record UpdateShareRequest(string Permission);

// Represents a single share entry returned to the client
public record ShareDto(int UserId, string Username, string Permission);