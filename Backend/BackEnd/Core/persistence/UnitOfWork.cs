// =============================================================================
// File:        UnitOfWork.cs
// Author:      Mart Sebastian
// Description: Implements the IUnitOfWork interface to encapsulate operations 
//              within a single transactional boundary. Orchestrates the context 
//              lifecycle and coordinates individual repository instances 
//              sharing a unified database session state.
// =============================================================================

using Core.business.interfaces;
using Core.persistence.repositories.implementations;

namespace Core.persistence;

public class UnitOfWork : IUnitOfWork
{
    // The unified database context backing all state transformations
    private readonly AppDbContext _context;

    // Public exposure of the concrete user repository layer instance
    public IUserRepository Users { get; }
    
    // Public exposure of the concrete document repository layer instance
    public IDocumentRepository Documents { get; }
    
    // Public exposure of the concrete joint access repository layer instance
    public IUserDocumentRepository SharedDocuments { get; }

    // Initializes repositories by tracking and forwarding the explicit context reference
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        Documents = new DocumentRepository(_context);
        SharedDocuments = new UserDocumentRepository(_context);
    }

    // Flushes all pending repository mutations to the targeted storage mechanism
    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    // Releases all managed persistent tracking state and infrastructure dependencies
    public void Dispose()
    {
        _context.Dispose();   
    }
}