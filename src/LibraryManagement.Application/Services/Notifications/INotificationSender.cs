namespace LibraryManagement.Application.Services.Notifications;

/// <summary>
/// Базовий інтерфейс для всіх нотифікацій (ISP - Interface Segregation Principle)
/// </summary>
public interface INotificationSender
{
    Task SendAsync(CancellationToken cancellationToken = default);
}
