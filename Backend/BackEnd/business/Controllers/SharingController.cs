using BackEnd.business.DTOs;
using BackEnd.business.Exceptions;
using BackEnd.business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.business.Controllers;

[ApiController]
[Route("shared-docs/{shareId:int}/shares")]
[Authorize]
public class SharingController : AuthenticatedController
{
    private readonly IDocumentService _docs;

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
            var result = await _docs.GetSharesAsync(CurrentUserId, shareId);
            return Ok(result);
        }
        catch (NotFoundException ex)  { return NotFound(new { error = ex.Message }); }
        catch (ForbiddenException ex) { return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message }); }
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
            var result = await _docs.AddShareAsync(CurrentUserId, shareId, req);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (ValidationException ex) { return BadRequest(new { error = ex.Message }); }
        catch (NotFoundException ex)   { return NotFound(new { error = ex.Message }); }
        catch (ForbiddenException ex)  { return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message }); }
        catch (ConflictException ex)   { return Conflict(new { error = ex.Message }); }
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
            var result = await _docs.UpdateShareAsync(CurrentUserId, shareId, userId, req);
            return Ok(result);
        }
        catch (ValidationException ex) { return BadRequest(new { error = ex.Message }); }
        catch (NotFoundException ex)   { return NotFound(new { error = ex.Message }); }
        catch (ForbiddenException ex)  { return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message }); }
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
            await _docs.RevokeShareAsync(CurrentUserId, shareId, userId);
            return NoContent();
        }
        catch (NotFoundException ex)  { return NotFound(new { error = ex.Message }); }
        catch (ForbiddenException ex) { return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message }); }
    }
}
