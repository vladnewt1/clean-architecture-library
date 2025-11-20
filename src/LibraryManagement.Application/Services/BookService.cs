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
        var book = Book.Create(
            bookDto.Title,
            bookDto.Author,
            bookDto.ISBN,
            bookDto.PublishedYear,
            bookDto.Publisher,
            bookDto.Category,
            bookDto.Description,
            bookDto.PageCount,
            bookDto.Language,
            bookDto.TotalCopies,
            bookDto.Price,
            bookDto.CoverImageUrl
        );

        var createdBook = await _bookRepository.AddAsync(book);
        return MapToDto(createdBook);
    }

    public async Task UpdateAsync(int id, UpdateBookDto bookDto)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
            throw new Exception($"Book with id {id} not found");

        book.UpdateDetails(
            bookDto.Title,
            bookDto.Author,
            bookDto.Publisher,
            bookDto.Category,
            bookDto.Description,
            bookDto.PageCount,
            bookDto.Language,
            bookDto.Price,
            bookDto.CoverImageUrl
        );

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
