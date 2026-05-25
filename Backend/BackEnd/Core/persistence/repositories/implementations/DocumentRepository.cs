// =============================================================================
// File:        DocumentRepository.cs
// Author:      Mart Sebastian
// Description: Implements the IDocumentRepository interface for data access
//              operations on Document entities. Extends the generic Repository
//              base class and adds a document-specific query for retrieving
//              all documents belonging to a given owner.
// =============================================================================

using Core.business.interfaces;
using Core.persistence.entities;
using Microsoft.EntityFrameworkCore;

namespace Core.persistence.repositories.implementations;

public class DocumentRepository : Repository<Document>, IDocumentRepository
{
    // Pass the database context to the base Repository
    public DocumentRepository(AppDbContext context) : base(context) { }

    // Retrieves all documents where OwnerId matches the specified user
    public async Task<IEnumerable<Document>> GetDocumentsByOwnerIdAsync(int ownerId)
    {
        return await Queryable
            .Where<Document>(_context.Documents, d => d.OwnerId == ownerId)
            .ToListAsync();
    }

    // Not implemented in this repository — belongs to IUserRepository
    public Task<User?> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    // Not implemented in this repository — belongs to IUserRepository
    public Task<User?> GetByUsernameAsync(string username)
    {
        throw new NotImplementedException();
    }
}