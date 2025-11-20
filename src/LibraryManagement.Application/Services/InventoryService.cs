using LibraryManagement.Application.DTOs;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Interfaces;
using LibraryManagement.Domain.Services;

namespace LibraryManagement.Application.Services;

public interface IInventoryService
{
    Task<InventoryReportDto> GetInventoryReportAsync();
    Task AddBookCopiesAsync(int bookId, int quantity, string reason);
    Task RemoveBookCopiesAsync(int bookId, int quantity, string reason);
    Task<IEnumerable<BookDto>> GetLowStockBooksAsync(int threshold = 2);
    Task<IEnumerable<BookDto>> GetOutOfStockBooksAsync();
}

public class InventoryService : IInventoryService
{
    private readonly IBookRepository _bookRepository;
    private readonly InventoryDomainService _inventoryDomainService;
    private readonly IUnitOfWork _unitOfWork;

    public InventoryService(
        IBookRepository bookRepository,
        IUnitOfWork unitOfWork)
    {
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
        _inventoryDomainService = new InventoryDomainService();
    }

    public async Task<InventoryReportDto> GetInventoryReportAsync()
    {
        var books = await _bookRepository.GetAllAsync();
        var booksList = books.ToList();

        return new InventoryReportDto
        {
            TotalBooks = booksList.Sum(b => b.TotalCopies),
            AvailableBooks = booksList.Sum(b => b.AvailableCopies),
            BorrowedBooks = booksList.Sum(b => b.TotalCopies - b.AvailableCopies),
            TotalValue = _inventoryDomainService.CalculateInventoryValue(booksList),
            LowStockCount = _inventoryDomainService.GetLowStockBooks(booksList).Count(),
            OutOfStockCount = _inventoryDomainService.GetOutOfStockBooks(booksList).Count(),
            UniqueBooks = booksList.Count
        };
    }

    public async Task AddBookCopiesAsync(int bookId, int quantity, string reason)
    {
        var book = await _bookRepository.GetByIdAsync(bookId);
        if (book == null)
            throw new Exception($"Book with id {bookId} not found");

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            _inventoryDomainService.AddBookCopies(book, quantity, reason);
            await _bookRepository.UpdateAsync(book);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task RemoveBookCopiesAsync(int bookId, int quantity, string reason)
    {
        var book = await _bookRepository.GetByIdAsync(bookId);
        if (book == null)
            throw new Exception($"Book with id {bookId} not found");

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            _inventoryDomainService.RemoveBookCopies(book, quantity, reason);
            await _bookRepository.UpdateAsync(book);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<IEnumerable<BookDto>> GetLowStockBooksAsync(int threshold = 2)
    {
        var books = await _bookRepository.GetAllAsync();
        var lowStockBooks = _inventoryDomainService.GetLowStockBooks(books, threshold);
        
        return lowStockBooks.Select(MapToDto);
    }

    public async Task<IEnumerable<BookDto>> GetOutOfStockBooksAsync()
    {
        var books = await _bookRepository.GetAllAsync();
        var outOfStockBooks = _inventoryDomainService.GetOutOfStockBooks(books);
        
        return outOfStockBooks.Select(MapToDto);
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
