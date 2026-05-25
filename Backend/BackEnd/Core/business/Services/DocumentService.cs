// =============================================================================
// File:        DocumentService.cs
// Author:      Gorea Sabin-Gabriel
// Description: Defines the IDocumentService interface and its implementation.
//              DocumentService handles all business logic for shared documents
//              and their sharing permissions: creating, reading, updating, and
//              deleting documents, as well as managing per-user share entries.
//              All data access is performed through IUnitOfWork.
// =============================================================================

using Core.business.DTOs;
using Core.business.Exceptions;
using Core.business.interfaces;
using Core.persistence.entities;

namespace Core.business.Services;

// Contract for all document and sharing operations
public interface IDocumentService
{
    Task<IEnumerable<SharedDocSummaryDto>> GetSharedDocsAsync(int callerId);
    Task<SharedDocDetailDto>               GetSharedDocAsync(int callerId, int documentId);
    Task<SharedDocDetailDto>               CreateDocumentAsync(int callerId, CreateDocumentRequest req);
    Task<SharedDocDetailDto>               UpdateDocumentAsync(int callerId, int documentId, UpdateDocumentRequest req);
    Task                                   DeleteSharedDocAsync(int callerId, int documentId);
    Task<IEnumerable<ShareDto>>            GetSharesAsync(int callerId, int documentId);
    Task<ShareDto>                         AddShareAsync(int callerId, int documentId, CreateShareRequest req);
    Task<ShareDto>                         UpdateShareAsync(int callerId, int documentId, int targetUserId, UpdateShareRequest req);
    Task                                   RevokeShareAsync(int callerId, int documentId, int targetUserId);
}

public class DocumentService : IDocumentService
{
    // Unit of work provides access to all repositories and transaction management
    private readonly IUnitOfWork _uow;

    // Inject IUnitOfWork via constructor
    public DocumentService(IUnitOfWork uow) => _uow = uow;

    // Parses a permission string into the SharePermission enum; throws on invalid values
    private static SharePermission ParsePermission(string raw) =>
        raw switch
        {
            nameof(SharePermission.ReadOnly)  => SharePermission.ReadOnly,
            nameof(SharePermission.ReadWrite) => SharePermission.ReadWrite,
            _ => throw new ValidationException($"Invalid permission '{raw}'. Use ReadOnly or ReadWrite."),
        };

    // Maps a Document entity to its DTO representation
    private static DocumentDto ToDocumentDto(Document d) => new(
        d.Id, d.Title, d.Content, d.Version, d.CreatedAt, d.UpdatedAt);

    // Maps a UserDocument (share entry with document loaded) to a detailed DTO
    private static SharedDocDetailDto ToDetail(UserDocument ud) => new(
        ud.DocumentId,
        ud.PermissionLevel.ToString(),
        ToDocumentDto(ud.Document));

    // Maps a UserDocument (share entry with document loaded) to a summary DTO
    private static SharedDocSummaryDto ToSummary(UserDocument ud) => new(
        ud.DocumentId,
        ud.Document.Title,
        ud.PermissionLevel.ToString());

    // Retrieves a share entry (with document) for the caller, throwing if not found
    private async Task<UserDocument> GetShareOrThrowAsync(int callerId, int documentId)
    {
        var share = await _uow.SharedDocuments.GetByUserAndDocumentWithDocumentAsync(callerId, documentId)
                    ?? throw new NotFoundException($"Document {documentId} not found or not accessible.");
        return share;
    }

    // Returns a summary list of all documents shared with the caller
    public async Task<IEnumerable<SharedDocSummaryDto>> GetSharedDocsAsync(int callerId)
    {
        var shares = await _uow.SharedDocuments.GetAllByUserIdAsync(callerId);
        return shares.Select(ToSummary);
    }

    // Returns the full details of a single shared document accessible by the caller
    public async Task<SharedDocDetailDto> GetSharedDocAsync(int callerId, int documentId)
    {
        var share = await GetShareOrThrowAsync(callerId, documentId);
        return ToDetail(share);
    }

    // Creates a new document and automatically grants the caller Owner-level access
    public async Task<SharedDocDetailDto> CreateDocumentAsync(int callerId, CreateDocumentRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Title))
            throw new ValidationException("Title is required.");

        var now = DateTime.UtcNow;

        // Build the new document entity
        var document = new Document
        {
            Title     = req.Title,
            Content   = req.Content ?? string.Empty,
            Version   = 1,
            OwnerId   = callerId,
            CreatedAt = now,
            UpdatedAt = now,
        };

        await _uow.Documents.AddAsync(document);
        await _uow.CompleteAsync(); // Persist document to obtain its generated ID

        // Automatically create an Owner share entry for the creator
        var ownerShare = new UserDocument
        {
            UserId          = callerId,
            DocumentId      = document.Id,
            PermissionLevel = SharePermission.Owner,
        };

        await _uow.SharedDocuments.AddAsync(ownerShare);
        await _uow.CompleteAsync(); // Persist the share entry

        // Reload the share with the document navigation property populated
        var loaded = await _uow.SharedDocuments.GetByUserAndDocumentWithDocumentAsync(callerId, document.Id);
        return ToDetail(loaded!);
    }

    // Updates document content; enforces write permission and optimistic concurrency via version check
    public async Task<SharedDocDetailDto> UpdateDocumentAsync(int callerId, int documentId, UpdateDocumentRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Content))
            throw new ValidationException("Content is required.");

        var share = await GetShareOrThrowAsync(callerId, documentId);

        // ReadOnly users cannot modify the document
        if (share.PermissionLevel == SharePermission.ReadOnly)
            throw new ForbiddenException("You have ReadOnly permission.");

        var doc = share.Document;

        // Reject if the client's version does not match the current server version
        if (req.Version != doc.Version)
            throw new ConflictException($"Version conflict: client has v{req.Version}, server is at v{doc.Version}.");

        // Apply the update and increment the version counter
        doc.Content   = req.Content;
        doc.Version  += 1;
        doc.UpdatedAt = DateTime.UtcNow;

        _uow.Documents.Update(doc);
        await _uow.CompleteAsync();

        return ToDetail(share);
    }

    // Deletes a document (Owner) or removes the caller's own share entry (non-Owner)
    public async Task DeleteSharedDocAsync(int callerId, int documentId)
    {
        var share = await GetShareOrThrowAsync(callerId, documentId);

        if (share.PermissionLevel == SharePermission.Owner)
        {
            // Owner deletion: remove all share entries first, then delete the document
            var allShares = await _uow.SharedDocuments.GetAllByDocumentIdAsync(documentId);
            foreach (var s in allShares)
                _uow.SharedDocuments.Remove(s);
            await _uow.CompleteAsync();

            _uow.Documents.Remove(share.Document);
        }
        else
        {
            // Non-owner: only remove the caller's own share entry (unshare)
            _uow.SharedDocuments.Remove(share);
        }

        await _uow.CompleteAsync();
    }

    // Returns all non-Owner share entries for a document; only the Owner may call this
    public async Task<IEnumerable<ShareDto>> GetSharesAsync(int callerId, int documentId)
    {
        var callerShare = await GetShareOrThrowAsync(callerId, documentId);

        if (callerShare.PermissionLevel != SharePermission.Owner)
            throw new ForbiddenException("Only the Owner can view shares.");

        var allShares = await _uow.SharedDocuments.GetAllByDocumentIdAsync(documentId);

        // Build the result list, resolving each user's username
        var result = new List<ShareDto>();
        foreach (var s in allShares.Where(s => s.PermissionLevel != SharePermission.Owner))
        {
            var user = await _uow.Users.GetByIdAsync(s.UserId)
                       ?? throw new NotFoundException($"User {s.UserId} not found.");
            result.Add(new ShareDto(s.UserId, user.Username, s.PermissionLevel.ToString()));
        }

        return result;
    }

    // Grants a new user access to the document; only the Owner may share
    public async Task<ShareDto> AddShareAsync(int callerId, int documentId, CreateShareRequest req)
    {
        var callerShare = await GetShareOrThrowAsync(callerId, documentId);

        if (callerShare.PermissionLevel != SharePermission.Owner)
            throw new ForbiddenException("Only the Owner can share the document.");

        var permission = ParsePermission(req.Permission);

        // Verify the target user exists
        var targetUser = await _uow.Users.GetByIdAsync(req.UserId)
                         ?? throw new NotFoundException($"User {req.UserId} not found.");

        // Prevent duplicate shares
        var existing = await _uow.SharedDocuments.GetByUserAndDocumentAsync(req.UserId, documentId);
        if (existing is not null)
            throw new ConflictException("Document is already shared with this user.");

        var newShare = new UserDocument
        {
            UserId          = req.UserId,
            DocumentId      = documentId,
            PermissionLevel = permission,
        };

        await _uow.SharedDocuments.AddAsync(newShare);
        await _uow.CompleteAsync();

        return new ShareDto(targetUser.Id, targetUser.Username, permission.ToString());
    }

    // Changes the permission level of an existing share; only the Owner may update permissions
    public async Task<ShareDto> UpdateShareAsync(int callerId, int documentId, int targetUserId, UpdateShareRequest req)
    {
        var callerShare = await GetShareOrThrowAsync(callerId, documentId);

        if (callerShare.PermissionLevel != SharePermission.Owner)
            throw new ForbiddenException("Only the Owner can change permissions.");

        var permission = ParsePermission(req.Permission);

        // Retrieve the share entry to update
        var targetShare = await _uow.SharedDocuments.GetByUserAndDocumentAsync(targetUserId, documentId)
                          ?? throw new NotFoundException($"Share for user {targetUserId} not found.");

        targetShare.PermissionLevel = permission;
        _uow.SharedDocuments.Update(targetShare);
        await _uow.CompleteAsync();

        // Resolve the target user's username for the response
        var targetUser = await _uow.Users.GetByIdAsync(targetUserId)
                         ?? throw new NotFoundException($"User {targetUserId} not found.");

        return new ShareDto(targetUser.Id, targetUser.Username, permission.ToString());
    }

    // Removes a user's access to the document; only the Owner may revoke shares
    public async Task RevokeShareAsync(int callerId, int documentId, int targetUserId)
    {
        var callerShare = await GetShareOrThrowAsync(callerId, documentId);

        if (callerShare.PermissionLevel != SharePermission.Owner)
            throw new ForbiddenException("Only the Owner can revoke shares.");

        // Retrieve the share entry to remove
        var targetShare = await _uow.SharedDocuments.GetByUserAndDocumentAsync(targetUserId, documentId)
                          ?? throw new NotFoundException($"Share for user {targetUserId} not found.");

        _uow.SharedDocuments.Remove(targetShare);
        await _uow.CompleteAsync();
    }
}