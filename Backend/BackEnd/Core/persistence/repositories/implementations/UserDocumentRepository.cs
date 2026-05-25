// =============================================================================
// File:        UserDocumentRepository.cs
// Author:      Mart Sebastian
// Description: Implements the IUserDocumentRepository interface for data access
//              operations on UserDocument (SharedDocuments) entities. Extends 
//              the generic Repository base class and provides query variations
//              for managing document access permissions and sharing links.
// =============================================================================

using Core.business.interfaces;
using Core.persistence.entities;
using Microsoft.EntityFrameworkCore;

namespace Core.persistence.repositories.implementations;

public class UserDocumentRepository : Repository<UserDocument>, IUserDocumentRepository {
    // Pass the database context to the base Repository class
    public UserDocumentRepository(AppDbContext context) : base(context) { }

    // Retrieves all shared document entries along with their main document details for a specific user
    public async Task<IEnumerable<UserDocument>> GetAllByUserIdAsync(int userId)
    {
        return await Queryable
            .Where<UserDocument>(_context.SharedDocuments
                .Include(ud => ud.Document), ud => ud.UserId == userId)
            .ToListAsync();
    }

    // Retrieves all shared document entries link data for a specific document identifier
    public async Task<IEnumerable<UserDocument>> GetAllByDocumentIdAsync(int documentId)
    {
        return await Queryable
            .Where<UserDocument>(_context.SharedDocuments, ud => ud.DocumentId == documentId)
            .ToListAsync();
    }

    // Finds a unique relation entry linking a specific user and a specific document
    public async Task<UserDocument?> GetByUserAndDocumentAsync(int userId, int documentId)
    {
        return await _context.SharedDocuments
            .FirstOrDefaultAsync(ud => ud.UserId == userId && ud.DocumentId == documentId);
    }

    // Finds a unique relation entry linking a user and a document, explicitly loading the related Document data
    public async Task<UserDocument?> GetByUserAndDocumentWithDocumentAsync(int userId, int documentId)
    {
        return await _context.SharedDocuments
            .Include(ud => ud.Document)
            .FirstOrDefaultAsync(ud => ud.UserId == userId && ud.DocumentId == documentId);
    }
}