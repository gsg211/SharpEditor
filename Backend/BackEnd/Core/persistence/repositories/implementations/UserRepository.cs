// =============================================================================
// File:        UserRepository.cs
// Author:      Mart Sebastian
// Description: Implements the IUserRepository interface for data access
//              operations on User entities. Extends the generic Repository
//              base class and provides explicit query mechanisms for looking
//              up unique accounts via email address or username.
// =============================================================================

using Core.business.interfaces;
using Core.persistence.entities;
using Microsoft.EntityFrameworkCore;

namespace Core.persistence.repositories.implementations;

public class UserRepository : Repository<User>, IUserRepository
{
    // Pass the database context to the generic base Repository class
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    // Finds a single user account associated with the given unique email address
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
    
    // Finds a single user account associated with the given unique username
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }
}