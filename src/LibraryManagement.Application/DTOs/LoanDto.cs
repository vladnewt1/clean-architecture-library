using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.DTOs;

public class LoanDto
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public LoanStatus Status { get; set; }
    public decimal? LateFee { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsOverdue { get; set; }
    public int DaysOverdue { get; set; }
}

public class CreateLoanDto
{
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public int LoanDurationDays { get; set; } = 14;
    public string Notes { get; set; } = string.Empty;
}

public class ReturnLoanDto
{
    public int LoanId { get; set; }
    public DateTime ReturnDate { get; set; }
    public string Notes { get; set; } = string.Empty;
}
