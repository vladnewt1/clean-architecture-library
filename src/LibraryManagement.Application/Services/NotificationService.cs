using Microsoft.Extensions.Logging;

namespace LibraryManagement.Application.Services;

public interface INotificationService
{
    Task SendBookBorrowedNotificationAsync(int memberId, string bookTitle, DateTime dueDate);
    Task SendBookReturnedNotificationAsync(int memberId, string bookTitle, decimal? lateFee);
    Task SendMemberRegisteredNotificationAsync(int memberId, string memberName, string libraryCardNumber);
    Task SendOverdueNotificationAsync(int memberId, string bookTitle, int daysOverdue);
    Task SendWelcomeEmailAsync(string email, string memberName, string libraryCardNumber);
}

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public async Task SendBookBorrowedNotificationAsync(int memberId, string bookTitle, DateTime dueDate)
    {
        _logger.LogInformation(
            "📚 Notification: Member {MemberId} borrowed '{BookTitle}'. Due date: {DueDate}",
            memberId, bookTitle, dueDate);
        
        // Тут можна додати реальну відправку email/SMS
        await Task.CompletedTask;
    }

    public async Task SendBookReturnedNotificationAsync(int memberId, string bookTitle, decimal? lateFee)
    {
        var feeMessage = lateFee.HasValue ? $" Late fee: {lateFee.Value:C}" : "";
        _logger.LogInformation(
            "✅ Notification: Member {MemberId} returned '{BookTitle}'.{FeeMessage}",
            memberId, bookTitle, feeMessage);
        
        await Task.CompletedTask;
    }

    public async Task SendMemberRegisteredNotificationAsync(int memberId, string memberName, string libraryCardNumber)
    {
        _logger.LogInformation(
            "🎉 Notification: New member registered! {MemberName} (ID: {MemberId}). Library Card: {CardNumber}",
            memberName, memberId, libraryCardNumber);
        
        await Task.CompletedTask;
    }

    public async Task SendOverdueNotificationAsync(int memberId, string bookTitle, int daysOverdue)
    {
        _logger.LogWarning(
            "⚠️ Notification: Member {MemberId} has overdue book '{BookTitle}' ({DaysOverdue} days overdue)",
            memberId, bookTitle, daysOverdue);
        
        await Task.CompletedTask;
    }

    public async Task SendWelcomeEmailAsync(string email, string memberName, string libraryCardNumber)
    {
        _logger.LogInformation(
            "📧 Welcome Email sent to {Email}: Welcome {MemberName}! Your library card number is {CardNumber}",
            email, memberName, libraryCardNumber);
        
        await Task.CompletedTask;
    }
}
