namespace LibraryManagement.Application.Services;

/// <summary>
/// Сервіс для генерації унікальних ідентифікаторів
/// Singleton - одна інстанція на весь додаток
/// </summary>
public interface IRequestIdGenerator
{
    string GenerateRequestId();
    int GetTotalRequestsCount();
}

public class RequestIdGenerator : IRequestIdGenerator
{
    private int _requestCounter = 0;
    private readonly string _instanceId;

    public RequestIdGenerator()
    {
        _instanceId = Guid.NewGuid().ToString()[..8];
    }

    public string GenerateRequestId()
    {
        Interlocked.Increment(ref _requestCounter);
        return $"REQ-{_instanceId}-{_requestCounter}";
    }

    public int GetTotalRequestsCount()
    {
        return _requestCounter;
    }
}
