// =============================================================================
// File:        SharedDocsController.cs
// Author:      Gorea Sabin-Gabriel
// Description: REST API controller that manages shared documents for the
//              authenticated user. Exposes CRUD endpoints under the
//              "/shared-docs" route and delegates all business logic to
//              IDocumentService. Service-layer exceptions are mapped to the
//              appropriate HTTP status codes.
// =============================================================================

using Core.business.DTOs;
using Core.business.Exceptions;
using Core.business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.business.Controllers;

[ApiController]
[Route("shared-docs")]
[Authorize] // All endpoints require a valid JWT
public class SharedDocsController : AuthenticatedController
{
    // Service responsible for document business logic
    private readonly IDocumentService _docs;

    // Inject IDocumentService via constructor
    public SharedDocsController(IDocumentService docs) => _docs = docs;

    // ── GET /shared-docs ──────────────────────────────────────────────────────

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SharedDocSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        // Retrieve all shared documents visible to the current user
        var result = await _docs.GetSharedDocsAsync(CurrentUserId);
        return Ok(result);
    }

    // ── GET /shared-docs/{shareId} ────────────────────────────────────────────

    [HttpGet("{shareId:int}")]
    [ProducesResponseType(typeof(SharedDocDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int shareId)
    {
        try
        {
            // Fetch a single shared document by its share ID
            var result = await _docs.GetSharedDocAsync(CurrentUserId, shareId);
            return Ok(result);
        }
        catch (NotFoundException ex)  { return NotFound(new { error = ex.Message }); }           // 404 – document not found
        catch (ForbiddenException ex) { return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message }); } // 403 – access denied
    }

    // ── POST /shared-docs ─────────────────────────────────────────────────────

    [HttpPost]
    [ProducesResponseType(typeof(SharedDocDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateDocumentRequest req)
    {
        try
        {
            // Create a new shared document owned by the current user
            var result = await _docs.CreateDocumentAsync(CurrentUserId, req);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (ValidationException ex) { return BadRequest(new { error = ex.Message }); } // 400 – invalid input
    }

    // ── PUT /shared-docs/{shareId} ────────────────────────────────────────────

    [HttpPut("{shareId:int}")]
    [ProducesResponseType(typeof(SharedDocDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int shareId, [FromBody] UpdateDocumentRequest req)
    {
        try
        {
            // Update the shared document identified by shareId
            var result = await _docs.UpdateDocumentAsync(CurrentUserId, shareId, req);
            return Ok(result);
        }
        catch (ValidationException ex) { return BadRequest(new { error = ex.Message }); }                                // 400 – invalid input
        catch (NotFoundException ex)   { return NotFound(new { error = ex.Message }); }                                  // 404 – document not found
        catch (ForbiddenException ex)  { return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message }); } // 403 – access denied
        catch (ConflictException ex)   { return Conflict(new { error = ex.Message }); }                                  // 409 – concurrency conflict
    }

    // ── DELETE /shared-docs/{shareId} ─────────────────────────────────────────

    [HttpDelete("{shareId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int shareId)
    {
        try
        {
            // Delete the shared document; no body is returned on success
            await _docs.DeleteSharedDocAsync(CurrentUserId, shareId);
            return NoContent();
        }
        catch (NotFoundException ex)  { return NotFound(new { error = ex.Message }); }           // 404 – document not found
        catch (ForbiddenException ex) { return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message }); } // 403 – access denied
    }
}