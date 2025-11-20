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
            Publisher = bookDto.Publisher,
            Category = bookDto.Category,
            Description = bookDto.Description,
            PageCount = bookDto.PageCount,
            Language = bookDto.Language,
            TotalCopies = bookDto.TotalCopies,
            AvailableCopies = bookDto.TotalCopies,
            Price = bookDto.Price,
            CoverImageUrl = bookDto.CoverImageUrl,
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
        book.Publisher = bookDto.Publisher;
        book.Category = bookDto.Category;
        book.Description = bookDto.Description;
        book.PageCount = bookDto.PageCount;
        book.TotalCopies = bookDto.TotalCopies;
        book.Price = bookDto.Price;
        book.CoverImageUrl = bookDto.CoverImageUrl;
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
            Publisher = book.Publisher,
            Category = book.Category,
            Description = book.Description,
            PageCount = book.PageCount,
            Language = book.Language,
            AvailableCopies = book.AvailableCopies,
            TotalCopies = book.TotalCopies,
            Price = book.Price,
            CoverImageUrl = book.CoverImageUrl
        };
    }
}
