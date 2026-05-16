using BackEnd.business.interfaces;
using BackEnd.persistence.entities;

namespace BackEnd.persistence.repositories.implementations;


public class DocumentRepository : Repository<Document>, IDocumentRepository {
    public DocumentRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Document>> GetDocumentsByAuthorIdAsync(int authorId) {
        // Exemplu de query specific
        // return await _context.Documents.Where(d => d.AuthorId == authorId).ToListAsync();
        throw new NotImplementedException();
    }
}