using BackEnd.business.DTOs;
using BackEnd.business.Exceptions;
using BackEnd.business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.business.Controllers;

[ApiController]
[Route("shared-docs")]
[Authorize]
public class SharedDocsController : AuthenticatedController
{
    private readonly IDocumentService _docs;

    public SharedDocsController(IDocumentService docs) => _docs = docs;

    // ── GET /shared-docs ──────────────────────────────────────────────────────

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SharedDocSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
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
            var result = await _docs.GetSharedDocAsync(CurrentUserId, shareId);
            return Ok(result);
        }
        catch (NotFoundException ex)  { return NotFound(new { error = ex.Message }); }
        catch (ForbiddenException ex) { return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message }); }
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
            var result = await _docs.CreateDocumentAsync(CurrentUserId, req);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (ValidationException ex) { return BadRequest(new { error = ex.Message }); }
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
            var result = await _docs.UpdateDocumentAsync(CurrentUserId, shareId, req);
            return Ok(result);
        }
        catch (ValidationException ex) { return BadRequest(new { error = ex.Message }); }
        catch (NotFoundException ex)   { return NotFound(new { error = ex.Message }); }
        catch (ForbiddenException ex)  { return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message }); }
        catch (ConflictException ex)   { return Conflict(new { error = ex.Message }); }
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
            await _docs.DeleteSharedDocAsync(CurrentUserId, shareId);
            return NoContent();
        }
        catch (NotFoundException ex)  { return NotFound(new { error = ex.Message }); }
        catch (ForbiddenException ex) { return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message }); }
    }
}
