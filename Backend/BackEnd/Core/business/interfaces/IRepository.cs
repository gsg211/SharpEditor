namespace Core.business.interfaces;

public interface IRepository<T> where T : class
{
    public Task<IEnumerable<T>> GetAllAsync();
    public Task<T> GetByIdAsync(int id);
    public Task AddAsync(T entity);
    public void Update(T entity);
}