namespace LibraryManagement.Application.DTOs;

public class MemberStatisticsDto
{
    public int MemberId { get; set; }
    public int TotalLoans { get; set; }
    public int ActiveLoans { get; set; }
    public int CompletedLoans { get; set; }
    public int OverdueLoans { get; set; }
    public decimal TotalLateFees { get; set; }
    public bool CanBorrowBooks { get; set; }
    public int RemainingBorrowCapacity { get; set; }
}
