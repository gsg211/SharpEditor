using BackEnd.business.DTOs;
using BackEnd.business.Exceptions;
using BackEnd.business.interfaces;
using BackEnd.persistence.entities;

namespace BackEnd.business.Services;

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
    private readonly IUnitOfWork _uow;

    public DocumentService(IUnitOfWork uow) => _uow = uow;

    private static SharePermission ParsePermission(string raw) =>
        raw switch
        {
            nameof(SharePermission.ReadOnly)  => SharePermission.ReadOnly,
            nameof(SharePermission.ReadWrite) => SharePermission.ReadWrite,
            _ => throw new ValidationException($"Invalid permission '{raw}'. Use ReadOnly or ReadWrite."),
        };

    private static DocumentDto ToDocumentDto(Document d) => new(
        d.Id, d.Title, d.Content, d.Version, d.CreatedAt, d.UpdatedAt);

    private static SharedDocDetailDto ToDetail(UserDocument ud) => new(
        ud.DocumentId,
        ud.PermissionLevel.ToString(),
        ToDocumentDto(ud.Document));

    private static SharedDocSummaryDto ToSummary(UserDocument ud) => new(
        ud.DocumentId,
        ud.Document.Title,
        ud.PermissionLevel.ToString());

    private async Task<UserDocument> GetShareOrThrowAsync(int callerId, int documentId)
    {
        var share = await _uow.SharedDocuments.GetByUserAndDocumentWithDocumentAsync(callerId, documentId)
                    ?? throw new NotFoundException($"Document {documentId} not found or not accessible.");
        return share;
    }

    public async Task<IEnumerable<SharedDocSummaryDto>> GetSharedDocsAsync(int callerId)
    {
        var shares = await _uow.SharedDocuments.GetAllByUserIdAsync(callerId);
        return shares.Select(ToSummary);
    }

    public async Task<SharedDocDetailDto> GetSharedDocAsync(int callerId, int documentId)
    {
        var share = await GetShareOrThrowAsync(callerId, documentId);
        return ToDetail(share);
    }

    public async Task<SharedDocDetailDto> CreateDocumentAsync(int callerId, CreateDocumentRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Title))
            throw new ValidationException("Title is required.");

        var now = DateTime.UtcNow;
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
        await _uow.CompleteAsync();

        var ownerShare = new UserDocument
        {
            UserId          = callerId,
            DocumentId      = document.Id,
            PermissionLevel = SharePermission.Owner,
        };

        await _uow.SharedDocuments.AddAsync(ownerShare);
        await _uow.CompleteAsync();

        var loaded = await _uow.SharedDocuments.GetByUserAndDocumentWithDocumentAsync(callerId, document.Id);
        return ToDetail(loaded!);
    }

    public async Task<SharedDocDetailDto> UpdateDocumentAsync(int callerId, int documentId, UpdateDocumentRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Content))
            throw new ValidationException("Content is required.");

        var share = await GetShareOrThrowAsync(callerId, documentId);

        if (share.PermissionLevel == SharePermission.ReadOnly)
            throw new ForbiddenException("You have ReadOnly permission.");

        var doc = share.Document;

        if (req.Version != doc.Version)
            throw new ConflictException($"Version conflict: client has v{req.Version}, server is at v{doc.Version}.");

        doc.Content   = req.Content;
        doc.Version  += 1;
        doc.UpdatedAt = DateTime.UtcNow;

        _uow.Documents.Update(doc);
        await _uow.CompleteAsync();

        return ToDetail(share);
    }

    public async Task DeleteSharedDocAsync(int callerId, int documentId)
    {
        var share = await GetShareOrThrowAsync(callerId, documentId);

        if (share.PermissionLevel == SharePermission.Owner)
        {
            var allShares = await _uow.SharedDocuments.GetAllByDocumentIdAsync(documentId);
            foreach (var s in allShares)
                _uow.SharedDocuments.Remove(s);
            await _uow.CompleteAsync();

            _uow.Documents.Remove(share.Document);
        }
        else
        {
            _uow.SharedDocuments.Remove(share);
        }

        await _uow.CompleteAsync();
    }

    public async Task<IEnumerable<ShareDto>> GetSharesAsync(int callerId, int documentId)
    {
        var callerShare = await GetShareOrThrowAsync(callerId, documentId);

        if (callerShare.PermissionLevel != SharePermission.Owner)
            throw new ForbiddenException("Only the Owner can view shares.");

        var allShares = await _uow.SharedDocuments.GetAllByDocumentIdAsync(documentId);

        var result = new List<ShareDto>();
        foreach (var s in allShares.Where(s => s.PermissionLevel != SharePermission.Owner))
        {
            var user = await _uow.Users.GetByIdAsync(s.UserId)
                       ?? throw new NotFoundException($"User {s.UserId} not found.");
            result.Add(new ShareDto(s.UserId, user.Username, s.PermissionLevel.ToString()));
        }

        return result;
    }

    public async Task<ShareDto> AddShareAsync(int callerId, int documentId, CreateShareRequest req)
    {
        var callerShare = await GetShareOrThrowAsync(callerId, documentId);

        if (callerShare.PermissionLevel != SharePermission.Owner)
            throw new ForbiddenException("Only the Owner can share the document.");

        var permission = ParsePermission(req.Permission);

        var targetUser = await _uow.Users.GetByIdAsync(req.UserId)
                         ?? throw new NotFoundException($"User {req.UserId} not found.");

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

    public async Task<ShareDto> UpdateShareAsync(int callerId, int documentId, int targetUserId, UpdateShareRequest req)
    {
        var callerShare = await GetShareOrThrowAsync(callerId, documentId);

        if (callerShare.PermissionLevel != SharePermission.Owner)
            throw new ForbiddenException("Only the Owner can change permissions.");

        var permission = ParsePermission(req.Permission);

        var targetShare = await _uow.SharedDocuments.GetByUserAndDocumentAsync(targetUserId, documentId)
                          ?? throw new NotFoundException($"Share for user {targetUserId} not found.");

        targetShare.PermissionLevel = permission;
        _uow.SharedDocuments.Update(targetShare);
        await _uow.CompleteAsync();

        var targetUser = await _uow.Users.GetByIdAsync(targetUserId)
                         ?? throw new NotFoundException($"User {targetUserId} not found.");

        return new ShareDto(targetUser.Id, targetUser.Username, permission.ToString());
    }

    public async Task RevokeShareAsync(int callerId, int documentId, int targetUserId)
    {
        var callerShare = await GetShareOrThrowAsync(callerId, documentId);

        if (callerShare.PermissionLevel != SharePermission.Owner)
            throw new ForbiddenException("Only the Owner can revoke shares.");

        var targetShare = await _uow.SharedDocuments.GetByUserAndDocumentAsync(targetUserId, documentId)
                          ?? throw new NotFoundException($"Share for user {targetUserId} not found.");

        _uow.SharedDocuments.Remove(targetShare);
        await _uow.CompleteAsync();
    }
}