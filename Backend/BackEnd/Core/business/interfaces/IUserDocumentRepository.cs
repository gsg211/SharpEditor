// =============================================================================
// File:        IUserDocumentRepository.cs
// Author:      Mart Sebastian
// Description: Defines the repository interface for UserDocument entities,
//              which represent the many-to-many relationship between users
//              and documents (sharing permissions). Extends the generic
//              IRepository with queries for looking up shares by user,
//              by document, or by both, and re-declares Remove for
//              explicit resolution.
// =============================================================================

using Core.persistence.entities;

namespace Core.business.interfaces;

public interface IUserDocumentRepository : IRepository<UserDocument>
{
    // Retrieves all share entries associated with a specific user
    Task<IEnumerable<UserDocument>> GetAllByUserIdAsync(int userId);

    // Retrieves all share entries associated with a specific document
    Task<IEnumerable<UserDocument>> GetAllByDocumentIdAsync(int documentId);

    // Retrieves a single share entry matching both user and document IDs
    Task<UserDocument?> GetByUserAndDocumentAsync(int userId, int documentId);

    // Retrieves a single share entry with the related Document entity eagerly loaded
    Task<UserDocument?> GetByUserAndDocumentWithDocumentAsync(int userId, int documentId);

    // Explicitly re-declared to ensure the UserDocument-specific Remove is
    // resolved correctly when called through this interface
    new void Remove(UserDocument entity);
}