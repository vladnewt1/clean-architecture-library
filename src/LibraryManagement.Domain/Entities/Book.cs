using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int PublishedYear { get; set; }
    public string Publisher { get; set; } = string.Empty;
    public BookCategory Category { get; set; }
    public string Description { get; set; } = string.Empty;
    public int PageCount { get; set; }
    public string Language { get; set; } = "Ukrainian";
    public int AvailableCopies { get; set; }
    public int TotalCopies { get; set; }
    public decimal Price { get; set; }
    public string CoverImageUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
