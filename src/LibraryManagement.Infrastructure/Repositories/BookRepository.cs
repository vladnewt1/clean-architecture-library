using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Interfaces;
using LibraryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Repositories;

/// <summary>
/// Concrete repository for Book entity
/// Inherits from generic Repository<Book> and implements IBookRepository with specific methods
/// </summary>
public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(LibraryDbContext context) : base(context)
    {
    }

    // Specific method for Book entity
    public async Task<IEnumerable<Book>> SearchByTitleOrAuthorAsync(string searchTerm)
    {
        return await _context.Books
            .Where(b => b.Title.Contains(searchTerm) || b.Author.Contains(searchTerm))
            .ToListAsync();
    }
}
