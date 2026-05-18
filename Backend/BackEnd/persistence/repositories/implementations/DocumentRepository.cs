using BackEnd.business.interfaces;
using BackEnd.persistence.entities;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.persistence.repositories.implementations;


public class DocumentRepository : Repository<Document>, IDocumentRepository {
    public DocumentRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Document>> GetDocumentsByOwnerIdAsync(int ownerId)
    {
        return await _context.Documents
            .Where(d => d.OwnerId == ownerId)
            .ToListAsync();
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        throw new NotImplementedException();
    }
}