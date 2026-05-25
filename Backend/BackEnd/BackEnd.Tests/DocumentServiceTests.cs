using Core.business.DTOs;
using Core.business.Exceptions;
using Core.business.interfaces;
using Core.business.Services;
using Core.persistence.entities;
using Moq;

namespace BackEnd.Tests;

public class DocumentServiceTests
{
    private readonly Mock<IUnitOfWork>              _uow     = new();
    private readonly Mock<IDocumentRepository>      _docs    = new();
    private readonly Mock<IUserDocumentRepository>  _shares  = new();
    private readonly Mock<IUserRepository>          _users   = new();
    private readonly IDocumentService               _sut;

    // Re-usable fixture data
    private static Document MakeDocument(int id = 1, int ownerId = 10) => new()
    {
        Id        = id,
        OwnerId   = ownerId,
        Title     = "Doc",
        Content   = "Hello",
        Version   = 1,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };

    private static UserDocument MakeShare(
        int userId, int docId, SharePermission perm, Document? doc = null) => new()
    {
        UserId          = userId,
        DocumentId      = docId,
        PermissionLevel = perm,
        Document        = doc ?? MakeDocument(docId),
    };

    public DocumentServiceTests()
    {
        _uow.Setup(u => u.Documents).Returns(_docs.Object);
        _uow.Setup(u => u.SharedDocuments).Returns(_shares.Object);
        _uow.Setup(u => u.Users).Returns(_users.Object);
        _sut = new DocumentService(_uow.Object);
    }

    // ── GetSharedDocs ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetSharedDocs_ReturnsSummariesForCaller()
    {
        var share = MakeShare(10, 1, SharePermission.Owner);
        _shares.Setup(s => s.GetAllByUserIdAsync(10))
               .ReturnsAsync(new[] { share });

        var result = (await _sut.GetSharedDocsAsync(10)).ToList();

        Assert.Single(result);
        Assert.Equal(1, result[0].ShareId);
    }

    // ── GetSharedDoc ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetSharedDoc_NotAccessible_ThrowsNotFoundException()
    {
        _shares.Setup(s => s.GetByUserAndDocumentWithDocumentAsync(10, 99))
               .ReturnsAsync((UserDocument?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.GetSharedDocAsync(10, 99));
    }

    [Fact]
    public async Task GetSharedDoc_Accessible_ReturnsDetail()
    {
        var share = MakeShare(10, 1, SharePermission.Owner);
        _shares.Setup(s => s.GetByUserAndDocumentWithDocumentAsync(10, 1))
               .ReturnsAsync(share);

        var result = await _sut.GetSharedDocAsync(10, 1);

        Assert.Equal(1, result.ShareId);
        Assert.Equal("Owner", result.Permission);
    }

    // ── CreateDocument ────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateDocument_EmptyTitle_ThrowsValidationException()
    {
        await Assert.ThrowsAsync<ValidationException>(
            () => _sut.CreateDocumentAsync(10, new CreateDocumentRequest("", null)));
    }

    [Fact]
    public async Task CreateDocument_Valid_PersistsDocumentAndOwnerShare()
    {
        var loadedShare = MakeShare(10, 0, SharePermission.Owner);
        _shares.Setup(s => s.GetByUserAndDocumentWithDocumentAsync(10, It.IsAny<int>()))
               .ReturnsAsync(loadedShare);

        await _sut.CreateDocumentAsync(10, new CreateDocumentRequest("My Doc", "content"));

        _docs.Verify(d => d.AddAsync(It.Is<Document>(doc =>
            doc.Title == "My Doc" && doc.OwnerId == 10 && doc.Version == 1)), Times.Once);

        _shares.Verify(s => s.AddAsync(It.Is<UserDocument>(ud =>
            ud.UserId == 10 && ud.PermissionLevel == SharePermission.Owner)), Times.Once);

        _uow.Verify(u => u.CompleteAsync(), Times.Exactly(2));
    }

    // ── UpdateDocument ────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateDocument_EmptyContent_ThrowsValidationException()
    {
        await Assert.ThrowsAsync<ValidationException>(
            () => _sut.UpdateDocumentAsync(10, 1, new UpdateDocumentRequest("", 1)));
    }

    [Fact]
    public async Task UpdateDocument_ReadOnlyPermission_ThrowsForbiddenException()
    {
        var share = MakeShare(10, 1, SharePermission.ReadOnly);
        _shares.Setup(s => s.GetByUserAndDocumentWithDocumentAsync(10, 1)).ReturnsAsync(share);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _sut.UpdateDocumentAsync(10, 1, new UpdateDocumentRequest("new content", 1)));
    }

    [Fact]
    public async Task UpdateDocument_VersionMismatch_ThrowsConflictException()
    {
        var doc   = MakeDocument(); // version = 1
        var share = MakeShare(10, 1, SharePermission.ReadWrite, doc);
        _shares.Setup(s => s.GetByUserAndDocumentWithDocumentAsync(10, 1)).ReturnsAsync(share);

        // client sends version 2 but server is at 1
        await Assert.ThrowsAsync<ConflictException>(
            () => _sut.UpdateDocumentAsync(10, 1, new UpdateDocumentRequest("new content", 2)));
    }

    [Fact]
    public async Task UpdateDocument_Valid_IncrementsVersion()
    {
        var doc   = MakeDocument();
        var share = MakeShare(10, 1, SharePermission.ReadWrite, doc);
        _shares.Setup(s => s.GetByUserAndDocumentWithDocumentAsync(10, 1)).ReturnsAsync(share);

        await _sut.UpdateDocumentAsync(10, 1, new UpdateDocumentRequest("updated", 1));

        Assert.Equal("updated", doc.Content);
        Assert.Equal(2, doc.Version);
        _docs.Verify(d => d.Update(doc), Times.Once);
    }

    // ── DeleteSharedDoc ───────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteSharedDoc_AsOwner_DeletesAllSharesAndDocument()
    {
        var doc        = MakeDocument();
        var ownerShare = MakeShare(10, 1, SharePermission.Owner, doc);
        var otherShare = MakeShare(20, 1, SharePermission.ReadOnly, doc);

        _shares.Setup(s => s.GetByUserAndDocumentWithDocumentAsync(10, 1)).ReturnsAsync(ownerShare);
        _shares.Setup(s => s.GetAllByDocumentIdAsync(1)).ReturnsAsync(new[] { ownerShare, otherShare });

        await _sut.DeleteSharedDocAsync(10, 1);

        _shares.Verify(s => s.Remove(ownerShare), Times.Once);
        _shares.Verify(s => s.Remove(otherShare), Times.Once);
        _docs.Verify(d => d.Remove(doc), Times.Once);
    }

    [Fact]
    public async Task DeleteSharedDoc_AsNonOwner_RemovesOnlyCallerShare()
    {
        var doc   = MakeDocument();
        var share = MakeShare(20, 1, SharePermission.ReadWrite, doc);
        _shares.Setup(s => s.GetByUserAndDocumentWithDocumentAsync(20, 1)).ReturnsAsync(share);

        await _sut.DeleteSharedDocAsync(20, 1);

        _shares.Verify(s => s.Remove(share), Times.Once);
        _docs.Verify(d => d.Remove(It.IsAny<Document>()), Times.Never);
    }

    // ── GetShares ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetShares_AsNonOwner_ThrowsForbiddenException()
    {
        var share = MakeShare(10, 1, SharePermission.ReadOnly);
        _shares.Setup(s => s.GetByUserAndDocumentWithDocumentAsync(10, 1)).ReturnsAsync(share);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _sut.GetSharesAsync(10, 1));
    }

    [Fact]
    public async Task GetShares_AsOwner_ExcludesOwnerFromList()
    {
        var doc        = MakeDocument();
        var ownerShare = MakeShare(10, 1, SharePermission.Owner, doc);
        var readerShare = MakeShare(20, 1, SharePermission.ReadOnly, doc);

        _shares.Setup(s => s.GetByUserAndDocumentWithDocumentAsync(10, 1)).ReturnsAsync(ownerShare);
        _shares.Setup(s => s.GetAllByDocumentIdAsync(1)).ReturnsAsync(new[] { ownerShare, readerShare });
        _users.Setup(u => u.GetByIdAsync(20)).ReturnsAsync(new User { Id = 20, Username = "bob" });

        var result = (await _sut.GetSharesAsync(10, 1)).ToList();

        Assert.Single(result);
        Assert.Equal(20, result[0].UserId);
    }

    // ── AddShare ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task AddShare_AsNonOwner_ThrowsForbiddenException()
    {
        var share = MakeShare(10, 1, SharePermission.ReadWrite);
        _shares.Setup(s => s.GetByUserAndDocumentWithDocumentAsync(10, 1)).ReturnsAsync(share);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _sut.AddShareAsync(10, 1, new CreateShareRequest(20, "ReadOnly")));
    }

    [Fact]
    public async Task AddShare_AlreadyShared_ThrowsConflictException()
    {
        var ownerShare = MakeShare(10, 1, SharePermission.Owner);
        _shares.Setup(s => s.GetByUserAndDocumentWithDocumentAsync(10, 1)).ReturnsAsync(ownerShare);
        _users.Setup(u => u.GetByIdAsync(20)).ReturnsAsync(new User { Id = 20 });
        _shares.Setup(s => s.GetByUserAndDocumentAsync(20, 1))
               .ReturnsAsync(MakeShare(20, 1, SharePermission.ReadOnly));

        await Assert.ThrowsAsync<ConflictException>(
            () => _sut.AddShareAsync(10, 1, new CreateShareRequest(20, "ReadOnly")));
    }

    [Fact]
    public async Task AddShare_InvalidPermission_ThrowsValidationException()
    {
        var ownerShare = MakeShare(10, 1, SharePermission.Owner);
        _shares.Setup(s => s.GetByUserAndDocumentWithDocumentAsync(10, 1)).ReturnsAsync(ownerShare);
        _users.Setup(u => u.GetByIdAsync(20)).ReturnsAsync(new User { Id = 20 });
        _shares.Setup(s => s.GetByUserAndDocumentAsync(20, 1)).ReturnsAsync((UserDocument?)null);

        await Assert.ThrowsAsync<ValidationException>(
            () => _sut.AddShareAsync(10, 1, new CreateShareRequest(20, "Admin")));
    }

    // ── RevokeShare ───────────────────────────────────────────────────────────

    [Fact]
    public async Task RevokeShare_AsOwner_RemovesTargetShare()
    {
        var ownerShare  = MakeShare(10, 1, SharePermission.Owner);
        var targetShare = MakeShare(20, 1, SharePermission.ReadOnly);
        _shares.Setup(s => s.GetByUserAndDocumentWithDocumentAsync(10, 1)).ReturnsAsync(ownerShare);
        _shares.Setup(s => s.GetByUserAndDocumentAsync(20, 1)).ReturnsAsync(targetShare);

        await _sut.RevokeShareAsync(10, 1, 20);

        _shares.Verify(s => s.Remove(targetShare), Times.Once);
        _uow.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task RevokeShare_TargetNotFound_ThrowsNotFoundException()
    {
        var ownerShare = MakeShare(10, 1, SharePermission.Owner);
        _shares.Setup(s => s.GetByUserAndDocumentWithDocumentAsync(10, 1)).ReturnsAsync(ownerShare);
        _shares.Setup(s => s.GetByUserAndDocumentAsync(99, 1)).ReturnsAsync((UserDocument?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.RevokeShareAsync(10, 1, 99));
    }
}
