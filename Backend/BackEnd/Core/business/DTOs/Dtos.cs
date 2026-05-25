namespace Core.business.DTOs;

// ── Auth ──────────────────────────────────────────────────────────────────────

public record RegisterRequest(string Username, string Email, string Password);

public record LoginRequest(string Email, string Password);

public record AuthResponse(string? Token, string? Message = null);
// ── Document / SharedDoc ──────────────────────────────────────────────────────

public record CreateDocumentRequest(string Title, string? Content);

public record UpdateDocumentRequest(string Content, int Version);

public record DocumentDto(int Id, string Title, string Content, int Version,
    DateTime CreatedAt, DateTime UpdatedAt);

public record SharedDocSummaryDto(int ShareId, string Title, string Permission);

public record SharedDocDetailDto(int ShareId, string Permission, DocumentDto Document);

// ── Sharing ───────────────────────────────────────────────────────────────────

public record CreateShareRequest(int UserId, string Permission);

public record UpdateShareRequest(string Permission);

public record ShareDto(int UserId, string Username, string Permission);
