using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Domain.Interfaces;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(int id);
    Task<IEnumerable<Book>> GetAllAsync();
    Task<Book> AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(int id);
    Task<IEnumerable<Book>> SearchByTitleOrAuthorAsync(string searchTerm);
}
