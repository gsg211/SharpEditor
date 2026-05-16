using BackEnd.persistence.entities;

namespace BackEnd.business.interfaces;

public interface IDocumentRepository : IRepository<Document> {
    Task<IEnumerable<Document>> GetDocumentsByAuthorIdAsync(int authorId);
}