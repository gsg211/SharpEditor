using Core.business.interfaces;
using Core.persistence.entities;
using Microsoft.EntityFrameworkCore;

namespace Core.persistence.repositories.implementations;


public class DocumentRepository : Repository<Document>, IDocumentRepository {
    public DocumentRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Document>> GetDocumentsByOwnerIdAsync(int ownerId)
    {
        return await Queryable
            .Where<Document>(_context.Documents, d => d.OwnerId == ownerId)
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