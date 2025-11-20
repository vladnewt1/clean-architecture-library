using Microsoft.Extensions.Logging;

namespace LibraryManagement.Application.Services;

public interface IAuditLogService
{
    Task LogEventAsync(string eventType, string description, object? data = null);
}

public class AuditLogService : IAuditLogService
{
    private readonly ILogger<AuditLogService> _logger;

    public AuditLogService(ILogger<AuditLogService> logger)
    {
        _logger = logger;
    }

    public async Task LogEventAsync(string eventType, string description, object? data = null)
    {
        var timestamp = DateTime.UtcNow;
        var dataJson = data != null ? System.Text.Json.JsonSerializer.Serialize(data) : "null";
        
        _logger.LogInformation(
            "📝 AUDIT LOG [{Timestamp}] {EventType}: {Description} | Data: {Data}",
            timestamp, eventType, description, dataJson);
        
        // Тут можна зберігати в окрему таблицю аудиту в БД
        await Task.CompletedTask;
    }
}
