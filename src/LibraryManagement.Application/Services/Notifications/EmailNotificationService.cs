using Microsoft.Extensions.Logging;

namespace LibraryManagement.Application.Services.Notifications;

/// <summary>
/// Реалізація Email нотифікацій (SRP - Single Responsibility Principle)
/// Цей клас відповідає ТІЛЬКИ за відправку email нотифікацій
/// </summary>
public class EmailNotificationService : IEmailNotificationService
{
    private readonly ILogger<EmailNotificationService> _logger;

    public EmailNotificationService(ILogger<EmailNotificationService> logger)
    {
        _logger = logger;
    }

    public async Task SendWelcomeEmailAsync(string email, string memberName, string libraryCardNumber)
    {
        _logger.LogInformation(
            "📧 Welcome Email sent to {Email}: Welcome {MemberName}! Your library card number is {CardNumber}",
            email, memberName, libraryCardNumber);
        
        // TODO: Реальна відправка email через SMTP або email service
        await Task.CompletedTask;
    }

    public async Task SendBookBorrowedEmailAsync(string email, string bookTitle, DateTime dueDate)
    {
        _logger.LogInformation(
            "📧 Book Borrowed Email sent to {Email}: You borrowed '{BookTitle}'. Due date: {DueDate}",
            email, bookTitle, dueDate);
        
        await Task.CompletedTask;
    }

    public async Task SendBookReturnedEmailAsync(string email, string bookTitle, decimal? lateFee)
    {
        var feeMessage = lateFee.HasValue ? $" Late fee: {lateFee.Value:C}" : "";
        _logger.LogInformation(
            "📧 Book Returned Email sent to {Email}: You returned '{BookTitle}'.{FeeMessage}",
            email, bookTitle, feeMessage);
        
        await Task.CompletedTask;
    }

    public async Task SendOverdueEmailAsync(string email, string bookTitle, int daysOverdue)
    {
        _logger.LogWarning(
            "📧 Overdue Email sent to {Email}: Your book '{BookTitle}' is {DaysOverdue} days overdue",
            email, bookTitle, daysOverdue);
        
        await Task.CompletedTask;
    }
}
