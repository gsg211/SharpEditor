// =============================================================================
// File:        IRepository.cs
// Author:      Mart Sebastian
// Description: Defines the generic repository interface that provides a
//              standard set of CRUD operations for any entity type. All
//              domain-specific repositories extend this interface to inherit
//              the base data-access contract.
// =============================================================================

namespace Core.business.interfaces;

public interface IRepository<T> where T : class
{
    // Retrieves all entities of type T from the data store
    public Task<IEnumerable<T>> GetAllAsync();

    // Retrieves a single entity by its primary key
    public Task<T> GetByIdAsync(int id);

    // Adds a new entity to the data store
    public Task AddAsync(T entity);

    // Marks an existing entity as modified so changes are persisted on next commit
    public void Update(T entity);
}