using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Events;

namespace LibraryManagement.Domain.Entities;

public class Book : AggregateRoot<int>
{
    public string Title { get; private set; }
    public string Author { get; private set; }
    public string ISBN { get; private set; }
    public int PublishedYear { get; private set; }
    public string Publisher { get; private set; }
    public BookCategory Category { get; private set; }
    public string Description { get; private set; }
    public int PageCount { get; private set; }
    public string Language { get; private set; }
    public int AvailableCopies { get; private set; }
    public int TotalCopies { get; private set; }
    public decimal Price { get; private set; }
    public string CoverImageUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation properties
    private readonly List<Loan> _loans = new();
    public IReadOnlyCollection<Loan> Loans => _loans.AsReadOnly();

    // Private constructor for EF Core
    private Book() 
    {
        Title = string.Empty;
        Author = string.Empty;
        ISBN = string.Empty;
        Publisher = string.Empty;
        Description = string.Empty;
        Language = "Ukrainian";
        CoverImageUrl = string.Empty;
    }

    private Book(
        string title,
        string author,
        string isbn,
        int publishedYear,
        string publisher,
        BookCategory category,
        string description,
        int pageCount,
        string language,
        int totalCopies,
        decimal price,
        string coverImageUrl)
    {
        Title = title;
        Author = author;
        ISBN = isbn;
        PublishedYear = publishedYear;
        Publisher = publisher;
        Category = category;
        Description = description;
        PageCount = pageCount;
        Language = language;
        TotalCopies = totalCopies;
        AvailableCopies = totalCopies;
        Price = price;
        CoverImageUrl = coverImageUrl;
        CreatedAt = DateTime.UtcNow;
    }

    public static Book Create(
        string title,
        string author,
        string isbn,
        int publishedYear,
        string publisher,
        BookCategory category,
        string description,
        int pageCount,
        string language,
        int totalCopies,
        decimal price,
        string coverImageUrl = "")
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        
        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Author cannot be empty", nameof(author));
        
        if (string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("ISBN cannot be empty", nameof(isbn));
        
        if (totalCopies < 0)
            throw new ArgumentException("Total copies cannot be negative", nameof(totalCopies));
        
        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        return new Book(title, author, isbn, publishedYear, publisher, category, 
            description, pageCount, language, totalCopies, price, coverImageUrl);
    }

    public void UpdateDetails(
        string title,
        string author,
        string publisher,
        BookCategory category,
        string description,
        int pageCount,
        string language,
        decimal price,
        string coverImageUrl)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        Title = title;
        Author = author;
        Publisher = publisher;
        Category = category;
        Description = description;
        PageCount = pageCount;
        Language = language;
        Price = price;
        CoverImageUrl = coverImageUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddCopies(int count)
    {
        if (count <= 0)
            throw new ArgumentException("Count must be positive", nameof(count));

        TotalCopies += count;
        AvailableCopies += count;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveCopies(int count)
    {
        if (count <= 0)
            throw new ArgumentException("Count must be positive", nameof(count));
        
        if (count > AvailableCopies)
            throw new InvalidOperationException("Cannot remove more copies than available");

        TotalCopies -= count;
        AvailableCopies -= count;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool CanBeBorrowed()
    {
        return AvailableCopies > 0;
    }

    public void BorrowCopy(int memberId)
    {
        if (!CanBeBorrowed())
            throw new InvalidOperationException("No copies available for borrowing");

        AvailableCopies--;
        UpdatedAt = DateTime.UtcNow;
        
        RaiseDomainEvent(new BookBorrowedEvent(Id, memberId, DateTime.UtcNow));
    }

    public void ReturnCopy(int memberId, decimal? lateFee)
    {
        if (AvailableCopies >= TotalCopies)
            throw new InvalidOperationException("Cannot return more copies than total");

        AvailableCopies++;
        UpdatedAt = DateTime.UtcNow;
        
        RaiseDomainEvent(new BookReturnedEvent(Id, memberId, DateTime.UtcNow, lateFee));
    }
}
