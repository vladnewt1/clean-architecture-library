using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Domain.Interfaces;

/// <summary>
/// Specific repository interface for Book entity
/// Inherits base CRUD operations from IRepository<Book> and adds specific methods
/// </summary>
public interface IBookRepository : IRepository<Book>
{
    /// <summary>
    /// Search books by title or author
    /// </summary>
    Task<IEnumerable<Book>> SearchByTitleOrAuthorAsync(string searchTerm);
}
