using Core.business.interfaces;
using Core.persistence.repositories.implementations;

namespace Core.persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IUserRepository Users { get; }
    public IDocumentRepository Documents { get; }
    public IUserDocumentRepository SharedDocuments { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        Documents = new DocumentRepository(_context);
        SharedDocuments = new UserDocumentRepository(_context);
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();   
    }
}