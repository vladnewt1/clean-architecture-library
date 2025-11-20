using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Interfaces;

public interface IBookService
{
    Task<BookDto?> GetByIdAsync(int id);
    Task<IEnumerable<BookDto>> GetAllAsync();
    Task<BookDto> CreateAsync(CreateBookDto bookDto);
    Task UpdateAsync(int id, UpdateBookDto bookDto);
    Task DeleteAsync(int id);
    Task<IEnumerable<BookDto>> SearchAsync(string searchTerm);
}
