using Core.persistence.entities;

namespace Core.business.interfaces;

public interface IUserRepository : IRepository<User> {
    // Aici poți adăuga metode specifice doar pentru User
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
}