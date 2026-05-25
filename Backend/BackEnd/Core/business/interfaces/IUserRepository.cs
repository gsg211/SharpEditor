// =============================================================================
// File:        IUserRepository.cs
// Author:      Mart Sebastian
// Description: Defines the repository interface for User entities. Extends
//              the generic IRepository with user-specific lookup methods
//              for retrieving accounts by email address or username.
// =============================================================================

using Core.persistence.entities;

namespace Core.business.interfaces;

public interface IUserRepository : IRepository<User>
{
    // Retrieves a user by their email address; returns null if not found
    Task<User?> GetByEmailAsync(string email);

    // Retrieves a user by their username; returns null if not found
    Task<User?> GetByUsernameAsync(string username);
}