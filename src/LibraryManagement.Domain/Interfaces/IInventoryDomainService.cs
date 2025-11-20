using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Domain.Interfaces;

/// <summary>
/// Інтерфейс для Inventory Domain Service (DIP - Dependency Inversion Principle)
/// </summary>
public interface IInventoryDomainService
{
    void AddBookCopies(Book book, int count);
    void RemoveBookCopies(Book book, int count);
    decimal CalculateInventoryValue(IEnumerable<Book> books);
    IEnumerable<Book> GetLowStockBooks(IEnumerable<Book> books, int threshold = 2);
    IEnumerable<Book> GetOutOfStockBooks(IEnumerable<Book> books);
}
