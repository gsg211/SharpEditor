using Core.persistence.entities;

namespace Core.business.interfaces;

public interface IDocumentRepository : IRepository<Document> {
    Task<IEnumerable<Document>> GetDocumentsByOwnerIdAsync(int ownerId);
    new void Remove(Document entity);

}