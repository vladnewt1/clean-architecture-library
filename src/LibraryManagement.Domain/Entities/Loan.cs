using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities;

public class Loan
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public LoanStatus Status { get; set; }
    public decimal? LateFee { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public bool IsOverdue => Status == LoanStatus.Active && DateTime.Now > DueDate;
    public int DaysOverdue => IsOverdue ? (DateTime.Now - DueDate).Days : 0;
}
