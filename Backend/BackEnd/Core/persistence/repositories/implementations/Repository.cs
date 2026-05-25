// =============================================================================
// File:        Repository.cs
// Author:      Mart Sebastian
// Description: Implements the generic IRepository interface providing common
//              data access operations (CRUD) for any entity type using 
//              Entity Framework Core.
// =============================================================================

using Core.business.interfaces;
using Microsoft.EntityFrameworkCore;

namespace Core.persistence.repositories.implementations;

public class Repository<T> : IRepository<T> where T : class
{
    // Database context accessible by derived repository classes
    protected readonly AppDbContext _context;

    // Initializes the repository with the shared database context
    public Repository(AppDbContext context)
    {
        _context = context;
    }

    // Retrieves all entities of type T from the database
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }
    
    // Finds a specific entity of type T by its primary key identifier
    public async Task<T> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    // Tracks a new entity of type T to be inserted into the database
    public async Task AddAsync(T entity) 
    {
        await _context.Set<T>().AddAsync(entity);
    }

    // Prepares an existing entity of type T for modification tracking
    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }
    
    // Prepares an existing entity of type T for deletion tracking
    public void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
}