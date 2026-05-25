// =============================================================================
// File:        SharingController.cs
// Author:      Gorea Sabin Gabriel
// Description: REST API controller that manages the sharing permissions of a
//              specific shared document. Exposes endpoints for listing,
//              adding, updating, and revoking per-user shares under the
//              "/shared-docs/{shareId}/shares" route. All business logic is
//              delegated to IDocumentService, and service-layer exceptions are
//              mapped to the appropriate HTTP status codes.
// =============================================================================

using Core.business.DTOs;
using Core.business.Exceptions;
using Core.business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.business.Controllers;

[ApiController]
[Route("shared-docs/{shareId:int}/shares")]
[Authorize] // All endpoints require a valid JWT
public class SharingController : AuthenticatedController
{
    // Service responsible for document and sharing business logic
    private readonly IDocumentService _docs;

    // Inject IDocumentService via constructor
    public SharingController(IDocumentService docs) => _docs = docs;

    // ── GET /shared-docs/{shareId}/shares ─────────────────────────────────────

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ShareDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetShares(int shareId)
    {
        try
        {
            // Retrieve all share entries for the specified document
            var result = await _docs.GetSharesAsync(CurrentUserId, shareId);
            return Ok(result);
        }
        catch (NotFoundException ex)  { return NotFound(new { error = ex.Message }); }            // 404 – document not found
        catch (ForbiddenException ex) { return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message }); } // 403 – access denied
    }

    // ── POST /shared-docs/{shareId}/shares ────────────────────────────────────

    [HttpPost]
    [ProducesResponseType(typeof(ShareDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddShare(int shareId, [FromBody] CreateShareRequest req)
    {
        try
        {
            // Grant access to a new user for the specified document
            var result = await _docs.AddShareAsync(CurrentUserId, shareId, req);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (ValidationException ex) { return BadRequest(new { error = ex.Message }); }                                // 400 – invalid input
        catch (NotFoundException ex)   { return NotFound(new { error = ex.Message }); }                                  // 404 – document or user not found
        catch (ForbiddenException ex)  { return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message }); } // 403 – access denied
        catch (ConflictException ex)   { return Conflict(new { error = ex.Message }); }                                  // 409 – share already exists
    }

    // ── PUT /shared-docs/{shareId}/shares/{userId} ────────────────────────────

    [HttpPut("{userId:int}")]
    [ProducesResponseType(typeof(ShareDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateShare(int shareId, int userId, [FromBody] UpdateShareRequest req)
    {
        try
        {
            // Update the share permissions of a specific user on the document
            var result = await _docs.UpdateShareAsync(CurrentUserId, shareId, userId, req);
            return Ok(result);
        }
        catch (ValidationException ex) { return BadRequest(new { error = ex.Message }); }                                // 400 – invalid input
        catch (NotFoundException ex)   { return NotFound(new { error = ex.Message }); }                                  // 404 – share or document not found
        catch (ForbiddenException ex)  { return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message }); } // 403 – access denied
    }

    // ── DELETE /shared-docs/{shareId}/shares/{userId} ─────────────────────────

    [HttpDelete("{userId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeShare(int shareId, int userId)
    {
        try
        {
            // Revoke the specified user's access to the document
            await _docs.RevokeShareAsync(CurrentUserId, shareId, userId);
            return NoContent(); // 204 – successfully revoked, no body returned
        }
        catch (NotFoundException ex)  { return NotFound(new { error = ex.Message }); }            // 404 – share or document not found
        catch (ForbiddenException ex) { return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message }); } // 403 – access denied
    }
}