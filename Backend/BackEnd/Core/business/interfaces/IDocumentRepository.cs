// =============================================================================
// File:        IDocumentRepository.cs
// Author:      Mart Sebastian
// Description: Defines the repository interface for Document entities.
//              Extends the generic IRepository with document-specific queries
//              and explicitly re-declares Remove to ensure the correct
//              override is used when deleting documents.
// =============================================================================

using Core.persistence.entities;

namespace Core.business.interfaces;

public interface IDocumentRepository : IRepository<Document>
{
    // Retrieves all documents owned by the specified user
    Task<IEnumerable<Document>> GetDocumentsByOwnerIdAsync(int ownerId);

    // Explicitly re-declared to ensure the Document-specific Remove is resolved
    // correctly when called through this interface
    new void Remove(Document entity);
}