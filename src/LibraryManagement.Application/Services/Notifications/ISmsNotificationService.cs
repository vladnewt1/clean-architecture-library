namespace LibraryManagement.Application.Services.Notifications;

/// <summary>
/// SMS нотифікації (SRP - окремий клас для окремої відповідальності)
/// </summary>
public interface ISmsNotificationService
{
    Task SendBookDueReminderAsync(string phoneNumber, string bookTitle, DateTime dueDate);
    Task SendOverdueSmsAsync(string phoneNumber, string bookTitle, int daysOverdue);
}
