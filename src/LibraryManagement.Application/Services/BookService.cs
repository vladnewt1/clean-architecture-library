using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Interfaces;

namespace LibraryManagement.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<BookDto?> GetByIdAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        return book == null ? null : MapToDto(book);
    }

    public async Task<IEnumerable<BookDto>> GetAllAsync()
    {
        var books = await _bookRepository.GetAllAsync();
        return books.Select(MapToDto);
    }

    public async Task<BookDto> CreateAsync(CreateBookDto bookDto)
    {
        var book = new Book
        {
            Title = bookDto.Title,
            Author = bookDto.Author,
            ISBN = bookDto.ISBN,
            PublishedYear = bookDto.PublishedYear,
            TotalCopies = bookDto.TotalCopies,
            AvailableCopies = bookDto.TotalCopies,
            CreatedAt = DateTime.UtcNow
        };

        var createdBook = await _bookRepository.AddAsync(book);
        return MapToDto(createdBook);
    }

    public async Task UpdateAsync(int id, UpdateBookDto bookDto)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
            throw new Exception($"Book with id {id} not found");

        book.Title = bookDto.Title;
        book.Author = bookDto.Author;
        book.PublishedYear = bookDto.PublishedYear;
        book.TotalCopies = bookDto.TotalCopies;
        book.UpdatedAt = DateTime.UtcNow;

        await _bookRepository.UpdateAsync(book);
    }

    public async Task DeleteAsync(int id)
    {
        await _bookRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<BookDto>> SearchAsync(string searchTerm)
    {
        var books = await _bookRepository.SearchByTitleOrAuthorAsync(searchTerm);
        return books.Select(MapToDto);
    }

    private static BookDto MapToDto(Book book)
    {
        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            ISBN = book.ISBN,
            PublishedYear = book.PublishedYear,
            AvailableCopies = book.AvailableCopies,
            TotalCopies = book.TotalCopies
        };
    }
}
