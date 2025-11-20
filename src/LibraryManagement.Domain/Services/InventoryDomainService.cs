using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Domain.Services;

public class InventoryDomainService
{
    public void AddBookCopies(Book book, int quantity, string reason)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        book.AddCopies(quantity);
    }

    public void RemoveBookCopies(Book book, int quantity, string reason)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (book.AvailableCopies < quantity)
            throw new InvalidOperationException(
                $"Cannot remove {quantity} copies. Only {book.AvailableCopies} available.");

        book.RemoveCopies(quantity);
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
