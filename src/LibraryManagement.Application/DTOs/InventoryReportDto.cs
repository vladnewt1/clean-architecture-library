namespace LibraryManagement.Application.DTOs;

public class InventoryReportDto
{
    public int TotalBooks { get; set; }
    public int AvailableBooks { get; set; }
    public int BorrowedBooks { get; set; }
    public decimal TotalValue { get; set; }
    public int LowStockCount { get; set; }
    public int OutOfStockCount { get; set; }
    public int UniqueBooks { get; set; }
}
