namespace LibraryManagement.Application.Services.Notifications;

/// <summary>
/// Email нотифікації (SRP - Single Responsibility)
/// </summary>
public interface IEmailNotificationService
{
    Task SendWelcomeEmailAsync(string email, string memberName, string libraryCardNumber);
    Task SendBookBorrowedEmailAsync(string email, string bookTitle, DateTime dueDate);
    Task SendBookReturnedEmailAsync(string email, string bookTitle, decimal? lateFee);
    Task SendOverdueEmailAsync(string email, string bookTitle, int daysOverdue);
}
