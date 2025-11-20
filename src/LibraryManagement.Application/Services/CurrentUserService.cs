namespace LibraryManagement.Application.Services;

/// <summary>
/// Сервіс для роботи з поточним користувачем в межах HTTP запиту
/// Scoped - нова інстанція для кожного HTTP запиту
/// </summary>
public interface ICurrentUserService
{
    string UserId { get; }
    string RequestId { get; }
    DateTime RequestStartTime { get; }
    TimeSpan GetRequestDuration();
}

public class CurrentUserService : ICurrentUserService
{
    public string UserId { get; }
    public string RequestId { get; }
    public DateTime RequestStartTime { get; }

    public CurrentUserService(IRequestIdGenerator requestIdGenerator)
    {
        UserId = "anonymous"; // В реальному додатку - з HttpContext
        RequestId = requestIdGenerator.GenerateRequestId();
        RequestStartTime = DateTime.UtcNow;
    }

    public TimeSpan GetRequestDuration()
    {
        return DateTime.UtcNow - RequestStartTime;
    }
}
