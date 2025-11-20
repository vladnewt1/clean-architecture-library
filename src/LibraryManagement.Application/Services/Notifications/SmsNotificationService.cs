using Microsoft.Extensions.Logging;

namespace LibraryManagement.Application.Services.Notifications;

/// <summary>
/// Реалізація SMS нотифікацій (SRP - Single Responsibility Principle)
/// Цей клас відповідає ТІЛЬКИ за відправку SMS нотифікацій
/// </summary>
public class SmsNotificationService : ISmsNotificationService
{
    private readonly ILogger<SmsNotificationService> _logger;

    public SmsNotificationService(ILogger<SmsNotificationService> logger)
    {
        _logger = logger;
    }

    public async Task SendBookDueReminderAsync(string phoneNumber, string bookTitle, DateTime dueDate)
    {
        _logger.LogInformation(
            "📱 SMS sent to {PhoneNumber}: Reminder - book '{BookTitle}' is due on {DueDate}",
            phoneNumber, bookTitle, dueDate);
        
        // TODO: Реальна відправка SMS через Twilio або інший SMS service
        await Task.CompletedTask;
    }

    public async Task SendOverdueSmsAsync(string phoneNumber, string bookTitle, int daysOverdue)
    {
        _logger.LogWarning(
            "📱 SMS sent to {PhoneNumber}: Book '{BookTitle}' is {DaysOverdue} days overdue. Please return it ASAP.",
            phoneNumber, bookTitle, daysOverdue);
        
        await Task.CompletedTask;
    }
}
