// =============================================================================
// File:        IUnitOfWork.cs
// Author:      Mart Sebastian
// Description: Defines the Unit of Work interface that groups all repository
//              instances into a single transactional scope. Provides a
//              single entry point for committing changes to the database and
//              ensures proper resource disposal via IDisposable.
// =============================================================================

namespace Core.business.interfaces;

public interface IUnitOfWork : IDisposable
{
    // Repository for user-related data access
    IUserRepository Users { get; }

    // Repository for document-related data access
    IDocumentRepository Documents { get; }

    // Repository for shared document (user-document join) data access
    IUserDocumentRepository SharedDocuments { get; }

    // Persists all pending changes across repositories to the database
    Task<int> CompleteAsync();
}