using BackEnd.persistence.entities;

namespace BackEnd.business.interfaces;

public interface IUserDocumentRepository : IRepository<UserDocument>
{
    Task<IEnumerable<UserDocument>> GetAllByUserIdAsync(int userId);
    Task<IEnumerable<UserDocument>> GetAllByDocumentIdAsync(int documentId);
    Task<UserDocument?> GetByUserAndDocumentAsync(int userId, int documentId);
    Task<UserDocument?> GetByUserAndDocumentWithDocumentAsync(int userId, int documentId);
    new void Remove(UserDocument entity);
}