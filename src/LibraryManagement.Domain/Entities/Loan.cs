using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Events;

namespace LibraryManagement.Domain.Entities;

public class Loan : AggregateRoot<int>
{
    private const decimal LateFeePerDay = 5m;
    private const int DefaultLoanPeriodDays = 14;

    public int BookId { get; private set; }
    public Book Book { get; private set; } = null!;
    public int MemberId { get; private set; }
    public Member Member { get; private set; } = null!;
    public DateTime LoanDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }
    public LoanStatus Status { get; private set; }
    public decimal? LateFee { get; private set; }
    public string Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    public bool IsOverdue => Status == LoanStatus.Active && DateTime.Now > DueDate;
    public int DaysOverdue => IsOverdue ? (DateTime.Now - DueDate).Days : 0;

    // Private constructor for EF Core
    private Loan() 
    {
        Notes = string.Empty;
    }

    private Loan(int bookId, int memberId, string notes = "")
    {
        BookId = bookId;
        MemberId = memberId;
        LoanDate = DateTime.UtcNow;
        DueDate = DateTime.UtcNow.AddDays(DefaultLoanPeriodDays);
        Status = LoanStatus.Active;
        Notes = notes;
        CreatedAt = DateTime.UtcNow;
    }

    public static Loan Create(int bookId, int memberId, string notes = "")
    {
        if (bookId <= 0)
            throw new ArgumentException("Invalid book ID", nameof(bookId));
        
        if (memberId <= 0)
            throw new ArgumentException("Invalid member ID", nameof(memberId));

        return new Loan(bookId, memberId, notes);
    }

    public void ReturnBook(string additionalNotes = "")
    {
        if (Status == LoanStatus.Returned)
            throw new InvalidOperationException("Book has already been returned");
        
        if (Status == LoanStatus.Lost)
            throw new InvalidOperationException("Cannot return a lost book");

        ReturnDate = DateTime.UtcNow;
        Status = LoanStatus.Returned;
        
        if (IsOverdue)
        {
            LateFee = CalculateLateFee();
        }

        if (!string.IsNullOrWhiteSpace(additionalNotes))
        {
            Notes = string.IsNullOrWhiteSpace(Notes) 
                ? additionalNotes 
                : $"{Notes}; {additionalNotes}";
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsLost()
    {
        if (Status == LoanStatus.Returned)
            throw new InvalidOperationException("Cannot mark returned book as lost");

        Status = LoanStatus.Lost;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ExtendDueDate(int days)
    {
        if (days <= 0)
            throw new ArgumentException("Extension days must be positive", nameof(days));
        
        if (Status != LoanStatus.Active)
            throw new InvalidOperationException("Can only extend active loans");

        DueDate = DueDate.AddDays(days);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus()
    {
        if (Status == LoanStatus.Active && DateTime.UtcNow > DueDate)
        {
            Status = LoanStatus.Overdue;
            UpdatedAt = DateTime.UtcNow;
            
            RaiseDomainEvent(new LoanOverdueEvent(Id, BookId, MemberId, DaysOverdue));
        }
    }

    public decimal CalculateLateFee()
    {
        if (!IsOverdue)
            return 0;

        return DaysOverdue * LateFeePerDay;
    }

    public void AddNote(string note)
    {
        if (string.IsNullOrWhiteSpace(note))
            return;

        Notes = string.IsNullOrWhiteSpace(Notes) 
            ? note 
            : $"{Notes}; {note}";
        UpdatedAt = DateTime.UtcNow;
    }
}
