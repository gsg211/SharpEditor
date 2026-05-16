using BackEnd.persistence.entities;
using BackEnd.business.interfaces;

namespace BackEnd.persistence.repositories.implementations;

public class UserDocumentRepository : Repository<UserDocument>, IUserDocumentRepository {
    public UserDocumentRepository(AppDbContext context) : base(context) { }
}