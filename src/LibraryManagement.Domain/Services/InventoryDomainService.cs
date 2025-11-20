using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Interfaces;

namespace LibraryManagement.Domain.Services;

/// <summary>
/// Domain Service для бізнес-логіки інвентаризації
/// Тепер дотримується DIP - має інтерфейс
/// </summary>
public class InventoryDomainService : IInventoryDomainService
{
    public void AddBookCopies(Book book, int count)
    {
        if (count <= 0)
            throw new ArgumentException("Count must be positive", nameof(count));

        book.AddCopies(count);
    }

    public void RemoveBookCopies(Book book, int count)
    {
        if (count <= 0)
            throw new ArgumentException("Count must be positive", nameof(count));

        if (book.AvailableCopies < count)
            throw new InvalidOperationException(
                $"Cannot remove {count} copies. Only {book.AvailableCopies} available.");

        book.RemoveCopies(count);
    }

    public bool IsBookAvailableForBorrowing(Book book)
    {
        return book.CanBeBorrowed();
    }

    public int GetBorrowedCopiesCount(Book book)
    {
        return book.TotalCopies - book.AvailableCopies;
    }

    public decimal CalculateInventoryValue(IEnumerable<Book> books)
    {
        return books.Sum(b => b.Price * b.TotalCopies);
    }

    public IEnumerable<Book> GetLowStockBooks(IEnumerable<Book> books, int threshold = 2)
    {
        return books.Where(b => b.AvailableCopies <= threshold && b.AvailableCopies > 0);
    }

    public IEnumerable<Book> GetOutOfStockBooks(IEnumerable<Book> books)
    {
        return books.Where(b => b.AvailableCopies == 0);
    }
}
