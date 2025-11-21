namespace LibraryManagement.Domain.Interfaces;

/// <summary>
/// Generic repository interface for basic CRUD operations
/// </summary>
/// <typeparam name="T">Entity type (reference type)</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Get entity by its identifier
    /// </summary>
    Task<T?> GetByIdAsync(int id);
    
    /// <summary>
    /// Get all entities
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();
    
    /// <summary>
    /// Add new entity
    /// </summary>
    Task<T> AddAsync(T entity);
    
    /// <summary>
    /// Update existing entity
    /// </summary>
    Task UpdateAsync(T entity);
    
    /// <summary>
    /// Delete entity by its identifier
    /// </summary>
    Task DeleteAsync(int id);
}
